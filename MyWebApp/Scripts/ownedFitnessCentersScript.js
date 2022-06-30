$(document).ready(function () {
    var userId = sessionStorage.getItem("userId");
    var userType = ["POSETILAC", "TRENER", "VLASNIK"];
    var userIsOwner = false;
    var isNameValid = false;
    var isStreetValid = false;
    var isNumberValid = false;
    var isPlaceValid = false;
    var isZipValid = false;
    var isMonthlySubValid = false;
    var isYearlySubValid = false;
    var isTrainingCostValid = false;
    var isGroupTrainingCostValid = false;
    var isPersonalTrainingCostValid = false;
    var fitnessCenterId = "";


    $("#fitnessCentersTableDiv").html("<h1>Nema pravo da pristupite ovom sadrzaju</h1>");
    $("#createFitnessCenterButton").hide();
    $("#editFitnessCenterButton").hide();
    $("#newFitnessCenterTable").hide();
    $("#inputFields").hide(); 

    GenerateYearCreatedOptions();

    function GenerateYearCreatedOptions() {
        for (let i = 2022; i > 1980; i--) {
            $("#fitnessCenterYear").append('<option>' + i + '</option>');
        }
    }

    $("#showNewFitnessCenterTableButton").click(function () {
        if ($("#showNewFitnessCenterTableButton").text() == "Sakrij") {
            $("#newFitnessCenterTable").hide();
            $("#createFitnessCenterButton").hide();
            $("#editFitnessCenterButton").hide();
            $("#showNewFitnessCenterTableButton").text("Novi fitnes centar");
            return;
        }
        $("#newFitnessCenterTable").show();
        $("#createFitnessCenterButton").show();
        $("#editFitnessCenterButton").hide();
        EmptyFitnessCenterTableFields();
        $("#showNewFitnessCenterTableButton").text("Sakrij");
    });

    GetOwner();

    function GetOwner() {
        if (userId != null && userId != "") {
            $.get("/api/users/getsessionuser", function (data, status) {
                // dobavimo usera, pa odredimo koji je tip 
                userIsOwner = userType[data.UserType] == "VLASNIK";
                // u zavisnosti od tipa prikazemo odredjene dugmice
                if (userIsOwner) {
                    $("#fitnessCentersTableDiv").html("");
                    $("#inputFields").show();
                    GenerateFitnessCenterTable();
                }
            }).fail(function (data) {
                alert(data.responseJSON);
            });
        }
    }

    function GenerateFitnessCenterTable() {
        $.get("/api/fitnesscenters/getowned", function (data, status) {
            if (data == null) {
                $("#fitnessCentersTableDiv").html("<h1>Nema pravo da pristupite ovom sadrzaju</h1>");
                return;
            }

            if (data.length == 0) {
                $("#fitnessCentersTableDiv").html("<h1>Nemate ni jedan fitnes centar</h1>");
                return;
            }

            let tableContent = "<table id='myTable' border='1'><caption align='center'>Moji fitnes centri</caption><tr><th>Naziv</th><th>Adresa</th><th>Godina otvaranja</th><th>Mesecna clanarina</th><th>Godisnja clanarina</th><th>Cena jednog treninga</th><th>Cena grupnog treninga</th><th>Cena personalnog treninga</th></tr>";
            for (fitnessCenter in data) {
                tableContent += `<tr><td>${data[fitnessCenter].Name}</td><td>${data[fitnessCenter].Address}</td><td>${data[fitnessCenter].YearCreated}</td><td>${data[fitnessCenter].MonthlySubscription}</td><td>${data[fitnessCenter].YearlySubscription}</td><td>${data[fitnessCenter].TrainingCost}</td><td>${data[fitnessCenter].GroupTrainingCost}</td><td>${data[fitnessCenter].PersonalTrainingCost}`;
                
                tableContent += `</td><td><button class='showEditFitnessCenterButton' name=${data[fitnessCenter].Id}>Izmeni</button></td><td><button class='deleteFitnessCenterButton' name=${data[fitnessCenter].Id}>Obrisi</button></td></tr>`;
            }
            tableContent += "</table>";
            $("#fitnessCentersTableDiv").html(tableContent);

            $(".showEditFitnessCenterButton").click(function (event) {
                if ($("#showNewFitnessCenterTableButton").text() != "Sakrij") {
                    fitnessCenterId = event.target.name;
                    EmptyFitnessCenterTableFields();
                    GenerateFitnessCenterEditTableFields();
                }
            });

            $(".deleteFitnessCenterButton").click(function (event) {
                if ($("#showNewFitnessCenterTableButton").text() == "Sakrij") {
                    $("#showNewFitnessCenterTableButton").trigger('click');
                }
                $.ajax(`/api/fitnesscenters/${event.target.name}`, {
                    method: 'DELETE',
                    success: function (result) {
                        alert(result);
                        GenerateFitnessCenterTable();
                    }
                }).fail(function (data) {
                    alert(data.responseJSON);
                });
            });
        }).fail(function (data) {
            alert(data.responseJSON);
        });
    }

    function EmptyFitnessCenterTableFields() {
        $("#fitnessCenterName").val("");
        $("#fitnessCenterName").css("border", "1px solid black");
        $("#invalidFitnessCenterName").html("");

        $("#fitnessCenterStreet").val("");
        $("#fitnessCenterStreet").css("border", "1px solid black");
        $("#invalidFitnessCenterStreet").html("");

        $("#fitnessCenterPlace").val("");
        $("#fitnessCenterPlace").css("border", "1px solid black");
        $("#invalidFitnessCenterPlace").html("");

        $("#fitnessCenterNumber").val("");
        $("#fitnessCenterNumber").css("border", "1px solid black");
        $("#invalidFitnessCenterNumber").html("");

        $("#fitnessCenterZip").val("");
        $("#fitnessCenterZip").css("border", "1px solid black");
        $("#invalidFitnessCenterZip").html("");

        $("#fitnessCenterMonthlySubscription").val("");
        $("#fitnessCenterMonthlySubscription").css("border", "1px solid black");
        $("#invalidFitnessCenterMonthlySubscription").html("");

        $("#fitnessCenterYearlySubscription").val("");
        $("#fitnessCenterYearlySubscription").css("border", "1px solid black");
        $("#invalidFitnessCenterYearlySubscription").html("");

        $("#fitnessCenterTrainingCost").val("");
        $("#fitnessCenterTrainingCost").css("border", "1px solid black");
        $("#invalidFitnessCenterTrainingCost").html("");

        $("#fitnessCenterGroupTrainingCost").val("");
        $("#fitnessCenterGroupTrainingCost").css("border", "1px solid black");
        $("#invalidFitnessCenterGroupTrainingCost").html("");

        $("#fitnessCenterPersonalTrainingCost").val("");
        $("#fitnessCenterPersonalTrainingCost").css("border", "1px solid black");
        $("#invalidFitnessCenterPersonalTrainingCost").html("");

        $("#fitnessCenterYear").val("2022");

        isNameValid = false;
        isStreetValid = false;
        isNumberValid = false;
        isPlaceValid = false;
        isZipValid = false;
        isMonthlySubValid = false;
        isYearlySubValid = false;
        isTrainingCostValid = false;
        isGroupTrainingCostValid = false;
        isPersonalTrainingCostValid = false;
    }

    function GenerateFitnessCenterEditTableFields() {
        $.get('/api/fitnesscenters', { 'id': fitnessCenterId }, function (result) {
            // prikaz radim ovde, jer ako ne nadjemo fitnes centar necu da pokaze polja
            $("#newFitnessCenterTable").show();
            $("#createFitnessCenterButton").hide();
            $("#editFitnessCenterButton").show();
            $("#showNewFitnessCenterTableButton").text("Sakrij");

            $("#fitnessCenterName").val(result.Name);
            let addressParts = result.Address.split(',');
            let streetParts = addressParts[0].split(' ');
            let street = "";
            for (let i in streetParts) {
                if (i == streetParts.length - 1) {
                    break;
                }
                street += streetParts[i] + " ";
            }
            street = street.trim();
            let number = streetParts[streetParts.length - 1];
            let place = addressParts[1].trim();
            let zip = addressParts[2].trim();
            $("#fitnessCenterStreet").val(street);
            $("#fitnessCenterPlace").val(place);
            $("#fitnessCenterNumber").val(number);
            $("#fitnessCenterZip").val(zip);
            $("#fitnessCenterMonthlySubscription").val(result.MonthlySubscription);
            $("#fitnessCenterYearlySubscription").val(result.YearlySubscription);
            $("#fitnessCenterTrainingCost").val(result.TrainingCost);
            $("#fitnessCenterGroupTrainingCost").val(result.GroupTrainingCost);
            $("#fitnessCenterPersonalTrainingCost").val(result.PersonalTrainingCost);
            $("#fitnessCenterYear").val(result.YearCreated);

            isNameValid = true;
            isStreetValid = true;
            isNumberValid = true;
            isPlaceValid = true;
            isZipValid = true;
            isMonthlySubValid = true;
            isYearlySubValid = true;
            isTrainingCostValid = true;
            isGroupTrainingCostValid = true;
            isPersonalTrainingCostValid = true;
        }).fail(function (data) {
            alert(data.responseJSON);
        });
    }

    $("#fitnessCenterName").focusout(function () {
        let name = $("#fitnessCenterName").val().trim();
        let nameReg = /^[A-Za-z0-9 ]{3,20}$/;
        let capitalReg = /[A-Z]/
        if (!capitalReg.test(name[0])) {
            isNameValid = false;
            $("#invalidFitnessCenterName").html("Naziv mora da pocinje velikim pocetnim slovom");
            $("#invalidFitnessCenterName").css("color", "red");
            $("#fitnessCenterName").css("border", "1px solid red");
            return;
        }
        if (!nameReg.test(name)) {
            isNameValid = false;
            let errorMessage = "";
            if (name.length < 3) {
                errorMessage = "Naziv mora da ima bar 3 slova";
            } else if (name.length > 20) {
                errorMessage = "Naziv ne moze da ima vise od 20 slova";
            } else {
                errorMessage = "Naziv ne moze da sadrzi specijalne karaktere";
            }
            $("#invalidFitnessCenterName").html(errorMessage);
            $("#invalidFitnessCenterName").css("color", "red");
            $("#fitnessCenterName").css("border", "1px solid red");
            return;
        }
        isNameValid = true;
        $("#fitnessCenterName").css("border", "1px solid black");
        $("#invalidFitnessCenterName").html("");
    });

    $("#fitnessCenterStreet").focusout(function () {
        let street = $("#fitnessCenterStreet").val().trim();
        let streetReg = /^[A-Za-z0-9 ]{3,20}$/;
        let capitalReg = /[A-Z]/
        if (!capitalReg.test(street[0])) {
            isStreetValid = false;
            $("#invalidFitnessCenterStreet").html("Naziv ulice mora da pocinje velikim pocetnim slovom");
            $("#invalidFitnessCenterStreet").css("color", "red");
            $("#fitnessCenterStreet").css("border", "1px solid red");
            return;
        }
        if (!streetReg.test(street)) {
            isStreetValid = false;
            let errorMessage = "";
            if (street.length < 3) {
                errorMessage = "Naziv ulice mora da ima bar 3 slova";
            } else if (street.length > 20) {
                errorMessage = "Naziv ulice ne moze da ima vise od 20 slova";
            } else {
                errorMessage = "Naziv ulice ne moze da sadrzi specijalne karaktere";
            }
            $("#invalidFitnessCenterStreet").html(errorMessage);
            $("#invalidFitnessCenterStreet").css("color", "red");
            $("#fitnessCenterStreet").css("border", "1px solid red");
            return;
        }
        isStreetValid = true;
        $("#fitnessCenterStreet").css("border", "1px solid black");
        $("#invalidFitnessCenterStreet").html("");
    });
    
    $("#fitnessCenterPlace").focusout(function () {
        let place = $("#fitnessCenterPlace").val().trim();
        let placeReg = /^[A-Za-z0-9 ]{3,20}$/;
        let capitalReg = /[A-Z]/
        if (!capitalReg.test(place[0])) {
            isPlaceValid = false;
            $("#invalidFitnessCenterPlace").html("Naziv mora da pocinje velikim pocetnim slovom");
            $("#invalidFitnessCenterPlace").css("color", "red");
            $("#fitnessCenterPlace").css("border", "1px solid red");
            return;
        }
        if (!placeReg.test(place)) {
            isPlaceValid = false;
            let errorMessage = "";
            if (place.length < 3) {
                errorMessage = "Naziv mora da ima bar 3 slova";
            } else if (place.length > 20) {
                errorMessage = "Naziv ne moze da ima vise od 20 slova";
            } else {
                errorMessage = "Naziv ne moze da sadrzi specijalne karaktere";
            }
            $("#invalidFitnessCenterPlace").html(errorMessage);
            $("#invalidFitnessCenterPlace").css("color", "red");
            $("#fitnessCenterPlace").css("border", "1px solid red");
            return;
        }
        isPlaceValid = true;
        $("#fitnessCenterPlace").css("border", "1px solid black");
        $("#invalidFitnessCenterPlace").html("");
    });

    $("#fitnessCenterNumber").focusout(function () {
        let number = $("#fitnessCenterNumber").val();
        if (isNaN(number) || number == "") {
            isNumberValid = false;
            $("#fitnessCenterNumber").css("border", "1px solid red");
            $("#invalidFitnessCenterNumber").html("Morate uneti broj");
            $("#invalidFitnessCenterNumber").css("color", "red");
            return;
        }
        if (number < 0 || number > 999) {
            isNumberValid = false;
            $("#fitnessCenterNumber").css("border", "1px solid red");
            $("#invalidFitnessCenterNumber").html("Uneti broj mora da je veci od 0 i manji od 999");
            $("#invalidFitnessCenterNumber").css("color", "red");
            return;
        }
        isNumberValid = true;
        $("#fitnessCenterNumber").css("border", "1px solid black");
        $("#invalidFitnessCenterNumber").html("");
    });

    $("#fitnessCenterZip").focusout(function () {
        let zip = $("#fitnessCenterZip").val();
        if (isNaN(zip) || zip == "") {
            isZipValid = false;
            $("#fitnessCenterZip").css("border", "1px solid red");
            $("#invalidFitnessCenterZip").html("Morate uneti broj");
            $("#invalidFitnessCenterZip").css("color", "red");
            return;
        }
        if (zip.length != 5) {
            isZipValid = false;
            $("#fitnessCenterZip").css("border", "1px solid red");
            $("#invalidFitnessCenterZip").html("Morate uneti validan postanski broj(5 cifara)");
            $("#invalidFitnessCenterZip").css("color", "red");
            return;
        }
        isZipValid = true;
        $("#fitnessCenterZip").css("border", "1px solid black");
        $("#invalidFitnessCenterZip").html("");
    });

    $("#fitnessCenterMonthlySubscription").focusout(function () {
        let monthlySub = $("#fitnessCenterMonthlySubscription").val();
        if (isNaN(monthlySub) || monthlySub == "") {
            isMonthlySubValid = false;
            $("#fitnessCenterMonthlySubscription").css("border", "1px solid red");
            $("#invalidFitnessCenterMonthlySubscription").html("Morate uneti broj");
            $("#invalidFitnessCenterMonthlySubscription").css("color", "red");
            return;
        }
        if (monthlySub < 0 || monthlySub > 1000000) {
            isMonthlySubValid = false;
            $("#fitnessCenterMonthlySubscription").css("border", "1px solid red");
            $("#invalidFitnessCenterMonthlySubscription").html("Uneti broj mora biti veci od 0 i manji od 1 000 000");
            $("#invalidFitnessCenterMonthlySubscription").css("color", "red");
            return;
        }
        isMonthlySubValid = true;
        $("#fitnessCenterMonthlySubscription").css("border", "1px solid black");
        $("#invalidFitnessCenterMonthlySubscription").html("");
    });

    $("#fitnessCenterYearlySubscription").focusout(function () {
        let yearlySub = $("#fitnessCenterYearlySubscription").val();
        if (isNaN(yearlySub) || yearlySub == "") {
            isYearlySubValid = false;
            $("#fitnessCenterYearlySubscription").css("border", "1px solid red");
            $("#invalidFitnessCenterYearlySubscription").html("Morate uneti broj");
            $("#invalidFitnessCenterYearlySubscription").css("color", "red");
            return;
        }
        if (yearlySub < 0 || yearlySub > 1000000) {
            isYearlySubValid = false;
            $("#fitnessCenterYearlySubscription").css("border", "1px solid red");
            $("#invalidFitnessCenterYearlySubscription").html("Uneti broj mora biti veci od 0 i manji od 1 000 000");
            $("#invalidFitnessCenterYearlySubscription").css("color", "red");
            return;
        }
        isYearlySubValid = true;
        $("#fitnessCenterYearlySubscription").css("border", "1px solid black");
        $("#invalidFitnessCenterYearlySubscription").html("");
    });

    $("#fitnessCenterTrainingCost").focusout(function () {
        let trainingCost = $("#fitnessCenterTrainingCost").val();
        if (isNaN(trainingCost) || trainingCost == "") {
            isTrainingCostValid = false;
            $("#fitnessCenterTrainingCost").css("border", "1px solid red");
            $("#invalidFitnessCenterTrainingCost").html("Morate uneti broj");
            $("#invalidFitnessCenterTrainingCost").css("color", "red");
            return;
        }
        if (trainingCost < 0 || trainingCost > 100000) {
            isTrainingCostValid = false;
            $("#fitnessCenterTrainingCost").css("border", "1px solid red");
            $("#invalidFitnessCenterTrainingCost").html("Uneti broj mora biti veci od 0 i manji od 100 000");
            $("#invalidFitnessCenterTrainingCost").css("color", "red");
            return;
        }
        isTrainingCostValid = true;
        $("#fitnessCenterTrainingCost").css("border", "1px solid black");
        $("#invalidFitnessCenterTrainingCost").html("");
    });

    $("#fitnessCenterGroupTrainingCost").focusout(function () {
        let groupTrainingCost = $("#fitnessCenterGroupTrainingCost").val();
        if (isNaN(groupTrainingCost) || groupTrainingCost == "") {
            isGroupTrainingCostValid = false;
            $("#fitnessCenterGroupTrainingCost").css("border", "1px solid red");
            $("#invalidFitnessCenterGroupTrainingCost").html("Morate uneti broj");
            $("#invalidFitnessCenterGroupTrainingCost").css("color", "red");
            return;
        }
        if (groupTrainingCost < 0 || groupTrainingCost > 100000) {
            isGroupTrainingCostValid = false;
            $("#fitnessCenterGroupTrainingCost").css("border", "1px solid red");
            $("#invalidFitnessCenterGroupTrainingCost").html("Uneti broj mora biti veci od 0 i manji od 100 000");
            $("#invalidFitnessCenterGroupTrainingCost").css("color", "red");
            return;
        }
        isGroupTrainingCostValid = true;
        $("#fitnessCenterGroupTrainingCost").css("border", "1px solid black");
        $("#invalidFitnessCenterGroupTrainingCost").html("");
    });

    $("#fitnessCenterPersonalTrainingCost").focusout(function () {
        let personalTrainingCost = $("#fitnessCenterPersonalTrainingCost").val();
        if (isNaN(personalTrainingCost) || personalTrainingCost == "") {
            isPersonalTrainingCostValid = false;
            $("#fitnessCenterPersonalTrainingCost").css("border", "1px solid red");
            $("#invalidFitnessCenterPersonalTrainingCost").html("Morate uneti broj");
            $("#invalidFitnessCenterPersonalTrainingCost").css("color", "red");
            return;
        }
        if (personalTrainingCost < 0 || personalTrainingCost > 100000) {
            isPersonalTrainingCostValid = false;
            $("#fitnessCenterPersonalTrainingCost").css("border", "1px solid red");
            $("#invalidFitnessCenterPersonalTrainingCost").html("Uneti broj mora biti veci od 0 i manji od 100 000");
            $("#invalidFitnessCenterPersonalTrainingCost").css("color", "red");
            return;
        }
        isPersonalTrainingCostValid = true;
        $("#fitnessCenterPersonalTrainingCost").css("border", "1px solid black");
        $("#invalidFitnessCenterPersonalTrainingCost").html("");
    });

    $("#createFitnessCenterButton").click(function () {
        if (!isNameValid || !isStreetValid || !isNumberValid || !isPlaceValid || !isZipValid || !isMonthlySubValid || !isYearlySubValid || !isTrainingCostValid || !isGroupTrainingCostValid || !isPersonalTrainingCostValid) {
            $("#fitnessCenterName").trigger('focusout');
            $("#fitnessCenterStreet").trigger('focusout');
            $("#fitnessCenterPlace").trigger('focusout');
            $("#fitnessCenterNumber").trigger('focusout');
            $("#fitnessCenterZip").trigger('focusout');
            $("#fitnessCenterMonthlySubscription").trigger('focusout');
            $("#fitnessCenterYearlySubscription").trigger('focusout');
            $("#fitnessCenterTrainingCost").trigger('focusout');
            $("#fitnessCenterGroupTrainingCost").trigger('focusout');
            $("#fitnessCenterPersonalTrainingCost").trigger('focusout');
            return;
        }
        let name = $("#fitnessCenterName").val();
        let street = $("#fitnessCenterStreet").val();
        let place = $("#fitnessCenterPlace").val();
        let number = $("#fitnessCenterNumber").val();
        let zip = $("#fitnessCenterZip").val();
        let address = street + " " + number + ", " + place + ", " + zip;
        let monthlySub = $("#fitnessCenterMonthlySubscription").val();
        let yearlySub = $("#fitnessCenterYearlySubscription").val();
        let trainingCost = $("#fitnessCenterTrainingCost").val();
        let groupTrainingCost = $("#fitnessCenterGroupTrainingCost").val();
        let personalTrainingCost = $("#fitnessCenterPersonalTrainingCost").val();
        let yearCreated = $("#fitnessCenterYear").val();
        $.post('/api/fitnesscenters', { 'name': name, 'address': address, 'yearCreated': yearCreated, 'monthlySubscription': monthlySub, 'yearlySubscription': yearlySub, 'trainingCost' : trainingCost, 'groupTrainingCost' : groupTrainingCost, 'personalTrainingCost' : personalTrainingCost},
            function (result) {
                alert(result);
                $("#showNewFitnessCenterTableButton").trigger('click');
                GenerateFitnessCenterTable();
            }
        ).fail(function (data) {
            alert(data.responseJSON);
        });
    });

    $("#editFitnessCenterButton").click(function () {
        if (!isNameValid || !isStreetValid || !isNumberValid || !isPlaceValid || !isZipValid || !isMonthlySubValid || !isYearlySubValid || !isTrainingCostValid || !isGroupTrainingCostValid || !isPersonalTrainingCostValid) {
            return;
        }
        let name = $("#fitnessCenterName").val();
        let street = $("#fitnessCenterStreet").val();
        let place = $("#fitnessCenterPlace").val();
        let number = $("#fitnessCenterNumber").val();
        let zip = $("#fitnessCenterZip").val();
        let address = street + " " + number + ", " + place + ", " + zip;
        let monthlySub = $("#fitnessCenterMonthlySubscription").val();
        let yearlySub = $("#fitnessCenterYearlySubscription").val();
        let trainingCost = $("#fitnessCenterTrainingCost").val();
        let groupTrainingCost = $("#fitnessCenterGroupTrainingCost").val();
        let personalTrainingCost = $("#fitnessCenterPersonalTrainingCost").val();
        let yearCreated = $("#fitnessCenterYear").val();
        $.ajax("/api/fitnesscenters", {
            method: 'PUT',
            data: { 'id': fitnessCenterId, 'name': name, 'address': address, 'yearCreated': yearCreated, 'monthlySubscription': monthlySub, 'yearlySubscription': yearlySub, 'trainingCost': trainingCost, 'groupTrainingCost': groupTrainingCost, 'personalTrainingCost': personalTrainingCost },
            success: function (result) {
                alert(result);
                $("#showNewFitnessCenterTableButton").trigger('click');
                GenerateFitnessCenterTable();
            }
        }).fail(function (data) {
            alert(data.responseJSON);
        });
    });
})