$(document).ready(function () {
    var id = $(location).attr('href').split('?')[1];
    var userType = ["POSETILAC", "TRENER", "VLASNIK"];
    var userId = sessionStorage.getItem("userId"); 
    var user = null;
    var userIsVisitor = false;
    var userIsOwner = false;

    // sakrij polje za komentar, pa ako jeste posetilac onda ga pokazi
    $("#commentDiv").hide();
    GetUser();

    // dobavljamo korisnika ako je ulogovan, jer onda generisemo odredjen sadrzaj za njega
    function GetUser() {
        if (userId != null && userId != "") {
            // imamo ulogovanog korisnika
            $.get("/api/users", { 'id': userId }, function (data, status) {
                user = data
                userIsVisitor = userType[user.UserType] == "POSETILAC"; // za sad se generise samo sadrzaj za posetioca
                userIsOwner = CheckIfUserOwner(user);
                // ako je korisnik ulogovan, prvo ga dobavimo, odredimo prava pristupa, pa onda kreiramo sadrzaj
                GenerateContent();

                if (userIsVisitor) {
                    $("#commentDiv").show();
                }
            });
        } else {
            // ako korisnik nije ulogovan samo kreiramo sadrzaj
            GenerateContent();
        }
    }

    function CheckIfUserOwner(user) {
        if (userType[user.UserType] != "VLASNIK") {
            return false;
        }
        for (let fitnessCenter in user.FitnessCentersOwned) {
            if (user.FitnessCentersOwned[fitnessCenter].Id == id) {
                return true;
            }
        }
        return false;
    }

    function GenerateContent() {
        // popunjavanje podataka o izabranom fitnes centru
        $.get("/api/fitnesscenters/", { 'id': id }, function (data, status) {
            $("#naziv").html(data.Name);
            $("#adresa").html(data.Address);
            $("#godina").html(data.YearCreated);
            $("#vlasnik").html(data.Owner.Name + " " + data.Owner.LastName);
            $("#mesecna").html(data.MonthlySubscription);
            $("#godisnja").html(data.YearlySubscription);
            $("#jedanTrening").html(data.TrainingCost);
            $("#grupniTrening").html(data.GroupTrainingCost);
            $("#personalniTrening").html(data.PersonalTrainingCost);

            GenerateGroupTrainingsTable();
            GenerateCommentsTable();
        }).fail(function (data) {
            alert(data.responseJSON);
        });
    }

    function GenerateGroupTrainingsTable() {
        $.get("/api/grouptrainings", { 'fitnessId': id }, function (data, status) {
            // pravi tabelu sa grupnim treninzima
            // ako je ulogovan korisnik posetilac, dodaje se jos jedna kolona koja sluzi za prijavljivanje na trening
            let tableContent = "<table border='1'><caption align='center'>Predstojeci grupni treninzi</caption><tr><th>Naziv</th><th>Vrsta treninga</th><th>Trajanje</th><th>Datum</th><th>Kapacitet</th><th>Broj prijavljenih</th>";
            if (userIsVisitor) {
                tableContent += "<th></th>";
            }
            tableContent += "</tr>";
            for (groupTraining in data) {
                let datum = data[groupTraining].DateOfTraining.slice(8, 10) + "/" + data[groupTraining].DateOfTraining.slice(5, 7) + "/" + data[groupTraining].DateOfTraining.slice(0, 4);// 2022-10-10
                let vreme = data[groupTraining].DateOfTraining.slice(11, 16);
                let date = datum + " " + vreme;
                tableContent += `<tr><td>${data[groupTraining].Name}</td><td>${data[groupTraining].TrainingType}</td><td>${data[groupTraining].Duration}</td><td>${date}</td><td>${data[groupTraining].VisitorCapacity}</td><td>${data[groupTraining].VisitorCount}</td>`;
                if (userIsVisitor) {
                    // dodavanje dugmeta ili poruke u novu kolonu za svaki red
                    tableContent += GenerateApplyContent(user, data[groupTraining]);
                }
                tableContent += "</tr>";
            }
            tableContent += "</table>";
            $("#fitnessCenterGroupTrainingsTable").html(tableContent);

            // event handler ako se klikne na dugme prijavi
            $(".applyButton").click(function (event) {
                let name = event.target.name; // ime dugmeta na koje se klikne je postavljeno da bude id grupnog treninga iz tog reda
                $.ajax("/api/grouptrainings/apply", {
                    method: 'PUT',
                    data: { 'id': name},    // akcija kontrolera prima objekat GroupTraining koji ce da ima bas ovaj id, a ostala polja su prazna
                    success: function (result) {
                        alert(result);
                        // moramo opet da dobavimo korisnika koji ima update-ovnu listu grupnih treninga i onda opet generisemo tabelu sa grupnim treninzima
                        GetUser();
                        GenerateGroupTrainingsTable();
                    }
                }).fail(function (data) {
                    alert(data.responseJSON);
                });
            });
        }).fail(function (data) {
            alert(data.responseJSON);
        });
    }

    // generise dodatnu kolonu za prijavljivanje
    // poziva se za svaki grupni trening
    function GenerateApplyContent(user, groupTraining) {
        let tableContent = "";
        let userAlreadyApplied = CheckIfUserApplied(user.VisitingGroupTrainings, groupTraining);
        if (userAlreadyApplied) {
            tableContent = `<td><font name=${groupTraining.Id} class='cannotApplyFont'>Prijavljen</font></td>`;
            return tableContent;
        }

        let noRoom = groupTraining.VisitorCapacity == groupTraining.VisitorCount;
        if (noRoom) {
            tableContent = `<td><font name=${groupTraining.Id} class='cannotApplyFont'>Popunjeno</font></td>`;
            return tableContent;
        }

        tableContent = `<td><button class='applyButton' name=${groupTraining.Id}>Prijavi se</button></td>`;
        return tableContent;
    }

    function CheckIfUserApplied(groupTrainingList, groupTraining) {
        for (let i in groupTrainingList) {
            if (groupTrainingList[i].Id == groupTraining.Id) {
                return true;
            }
        }
        return false;
    }

    function GenerateCommentsTable() {
        $.get("/api/comments", { 'fitnessId': id }, function (data, status) {
            /*
            let tableContent = "<table border='1'><caption align='center'>Komentari</caption><tr><th>Komentar</th><th>Ocena</th></tr>";
            for (comment in data) {
                tableContent += `<tr><td>${data[comment].Text}</td><td>${data[comment].Rating}</td></tr>`;
            }
            tableContent += "</table>";
            */
            let tableContent = "<h3>Komentari</h3><dl>"
            for (comment in data) {
                tableContent += `<dt>${data[comment].Creator.Username} : </dt><dd>${data[comment].Text}<br/>Ocena:${data[comment].Rating}`;
                // userIsOwner je true ako je korisnik VLASNIK i ako je bas vlasnik ovog fitnes centra
                // onda treba da moze da odobri/odbije komentare
                if (userIsOwner) {
                    tableContent += "<br/>" + GetCommentStatus(data[comment]);
                }
                tableContent += "</dd>";
            }
            tableContent += "</dl>";
            $("#fitnessCenterCommentsTable").html(tableContent);

            $(".approveButton").click(function (event) {
                let visible = event.target.textContent == "Odobri";  // vrati tekst koji se nalazi u button
                let commentId = event.target.name;
                $.ajax("/api/comments/", {
                    method: 'PUT',
                    data: { 'id': commentId, 'approved' : true, 'visible' : visible},
                    success: function (result) {
                        GenerateCommentsTable();
                    }
                }).fail(function (data) {
                    alert(data.responseJSON);
                });
            });
        });
    }

    $("#sendCommentButton").click(function () {
        let text = $("#commentText").val();
        let rating = $("#commentRating").val();
        $("#commentText").css("border", "1px solid black");
        if (text == "" || text.includes(";")) {
            $("#commentText").css("border", "1px solid red");
            return;
        }
        // u suprotnom salji
        $.post('/api/comments', {'text' : text, 'rating' : rating, 'fitnesscenterid' : id},
            function (result) {
                // result.responeJSON.Message je undefined ovde, ali dole moze
                // samo result je string koji posaljemo u Ok("text");
                alert(result);
                // izbrisi tekst u textarea i vrati ocenu na 1
                $("#commentText").val("");
                $("#commentRating").val("1");
            }
        ).fail(function (data) {
            alert(data.responseJSON);
        });
    });


    // ako je komentar odobren generise tekst, a ako nije generise dugmice
    function GetCommentStatus(comment) {
        let text = "";
        if (comment.Approved) {
            if (comment.Visible) {
                text += "<font color='green'>Odobren</font>";
                return text;
            }
            text += "<font color='red'>Odbijen</font>";
            return text;
        }
        text += `<button class='approveButton' name='${comment.Id}'>Odobri</button>&nbsp;<button class='approveButton' name='${comment.Id}'>Odbij</button>`;
        return text;
    }
})