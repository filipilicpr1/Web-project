$(document).ready(function () {
    var months = ['Januar', 'Februar', 'Mart', 'April', 'Maj', 'Jun', 'Jul', 'Avgust', 'Septembar', 'Oktobar', 'Novembar', 'Decembar'];
    var userId = sessionStorage.getItem("userId");
    var userType = ["POSETILAC", "TRENER", "VLASNIK"];
    var userIsTrainer = false;
    var isTrainingNameValid = false;
    var isTrainingTypeValid = false;
    var isTrainingDurationValid = false;
    var isTrainingCapacityValid = false;
    var isTrainingHourValid = true;
    var isTrainingMinuteValid = true;
    var isTrainingDateValid = false;
    var groupTrainingId = "";

    $("#groupTrainingsTableDiv").html("<h1>Nema pravo da pristupite ovom sadrzaju</h1>");
    $("#createTrainingButton").hide();
    $("#editTrainingButton").hide();
    $("#newTrainingTable").hide();
    $("#inputFields").hide();  // na kraju je vidljivo samo dugme , ali njega krijemo ako pristupa neulogovan korisnik

    InitiateYearMonthDay();

    function InitiateYearMonthDay() {
        GetOptionsForMonth();
        GetOptionsForYear();
        let year = $("#trainingYear").val();
        let month = $("#trainingMonth").val();
        GetOptionsForDay(year, month);
    }

    function GetOptionsForYear() {
        for (let i = 2022; i < 2033; i++) {
            $("#trainingYear").append('<option>' + i + '</option>');
        }
    }

    function GetOptionsForMonth() {
        for (let i in months) {
            $("#trainingMonth").append('<option>' + months[i] + '</option>');
        }
    }

    function GetOptionsForDay(year, month) {
        $("#trainingDay").empty();
        for (let i = 1; i < 29; i++) {
            $("#trainingDay").append('<option>' + i + '</option>');
        }

        if (month == "Februar" && !IsLeapYear(year)) {
            return;
        }

        $("#trainingDay").append('<option>29</option>');
        if (month == "Februar" && IsLeapYear(year)) {
            return;
        }

        $("#trainingDay").append('<option>30</option>');
        if (month == "April" || month == "Jun" || month == "Septembar" || month == "Novembar") {
            return;
        }
        $("#trainingDay").append('<option>31</option>');
    }

    function IsLeapYear(year) {
        if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0)) {
            return true;
        }
        return false;
    }

    $("#trainingYear").change(function (event) {
        let year = $("#trainingYear").val();
        let month = $("#trainingMonth").val();
        let day = $("#trainingDay").val();
        // ovo resetuje dan, pa proverimo da li novi select sadrzi stari dan
        GetOptionsForDay(year, month);
        var exists = false;
        $('#trainingDay  option').each(function () {
            if (this.value == day) {
                exists = true;
            }
        });
        if (exists) {
            $("#trainingDay").val(day);
        }
    });

    $("#trainingMonth").change(function () {
        let year = $("#trainingYear").val();
        let month = $("#trainingMonth").val();
        let day = $("#trainingDay").val();
        // ovo resetuje dan, pa proverimo da li novi select sadrzi stari dan
        GetOptionsForDay(year, month);
        var exists = false;
        $('#trainingDay  option').each(function () {
            if (this.value == day) {
                exists = true;
            }
        });
        if (exists) {
            $("#trainingDay").val(day);
        }
    });

    $("#showNewTrainingTableButton").click(function () {
        if ($("#showNewTrainingTableButton").text() == "Sakrij") {
            $("#newTrainingTable").hide();
            $("#createTrainingButton").hide();
            $("#editTrainingButton").hide();
            $("#showNewTrainingTableButton").text("Novi trening");
            return;
        }
        $("#newTrainingTable").show();
        $("#createTrainingButton").show();
        $("#editTrainingButton").hide();
        EmptyTrainingTableFields();
        $("#showNewTrainingTableButton").text("Sakrij");
    });

    GetTrainer();

    function GetTrainer() {
        if (userId != null && userId != "") {
            $.get("/api/users", { 'id': userId }, function (data, status) {
                // dobavimo usera, pa odredimo koji je tip 
                userIsTrainer = userType[data.UserType] == "TRENER";
                // u zavisnosti od tipa prikazemo odredjene dugmice
                if (userIsTrainer) {
                    $("#groupTrainingsTableDiv").html("");
                    $("#inputFields").show(); 
                    GenerateGroupTrainingTable();
                }
            }).fail(function (data) {
                alert(data.responseJSON);
            });
        }
    }
    
    function GenerateGroupTrainingTable() {
        $.get("/api/grouptrainings/trainedtrainings", function (data, status) {
            if (data == null) {
                $("#groupTrainingsTableDiv").html("<h1>Nema pravo da pristupite ovom sadrzaju</h1>");
                return;
            }

            if (data.length == 0) {
                $("#groupTrainingsTableDiv").html("<h1>Nemate ni jedan trening</h1>");
                return;
            }

            let tableContent = "<table id='myTable' border='1'><caption align='center'>Moji grupni treninzi</caption><tr><th>Fitnes centar</th><th>Naziv</th><th>Vrsta treninga</th><th>Trajanje</th><th>Datum</th><th>Kapacitet</th><th>Broj ucesnika</th></tr>";
            for (groupTraining in data) {
                let datum = data[groupTraining].DateOfTraining.slice(8, 10) + "/" + data[groupTraining].DateOfTraining.slice(5, 7) + "/" + data[groupTraining].DateOfTraining.slice(0, 4);// 2022-10-10
                let vreme = data[groupTraining].DateOfTraining.slice(11, 16);
                let date = datum + " " + vreme;
                tableContent += `<tr><td>${data[groupTraining].FitnessCenterLocation.Name}</td><td>${data[groupTraining].Name}</td><td>${data[groupTraining].TrainingType}</td><td>${data[groupTraining].Duration}</td><td>${date}</td><td>${data[groupTraining].VisitorCapacity}</td><td>${data[groupTraining].VisitorCount} `;
                if (data[groupTraining].VisitorCount > 0) {
                    tableContent += GetVisitorList(data[groupTraining]);
                }
                tableContent += `</td><td><button class='showEditTrainingButton' name=${data[groupTraining].Id}>Izmeni</button></td><td><button class='deleteTrainingButton' name=${data[groupTraining].Id}>Obrisi</button></td></tr>`;
            }
            tableContent += "</table>";
            $("#groupTrainingsTableDiv").html(tableContent);

            $(".showEditTrainingButton").click(function (event) {
                if ($("#showNewTrainingTableButton").text() != "Sakrij") {
                    EmptyTrainingTableFields();
                    groupTrainingId = event.target.name;
                    GenerateEditTrainingTableFields();
                }
            });

            $(".deleteTrainingButton").click(function (event) {
                if ($("#showNewTrainingTableButton").text() == "Sakrij") {
                    $("#showNewTrainingTableButton").trigger('click');
                }
                $.ajax(`/api/grouptrainings/${event.target.name}`, {
                    method: 'DELETE',
                    success: function (result) {
                        alert(result);
                        GenerateGroupTrainingTable();
                    }
                }).fail(function (data) {
                    alert(data.responseJSON);
                });
            });
        }).fail(function (data) {
            alert(data.responseJSON);
        });
    }

    function GetVisitorList(groupTraining){
        let text = "<select>";
        for(let visitor in groupTraining.Visitors){
            text += "<option>" + groupTraining.Visitors[visitor].Username + "</option>";
        }
        text += "</select>";
        return text;
    }

    function GenerateEditTrainingTableFields() {
        $.get('/api/grouptrainings', { 'id': groupTrainingId }, function (result) {
            if (!result.Upcoming) {
                alert("Moguce je izmeniti samo predstojece treninge");
                return;
            }
            // ovaj deo bi normalno isao gore, ali posto na frontu ima validacija da li je trening u buducnosti mora ovde
            $("#newTrainingTable").show();
            $("#createTrainingButton").hide();
            $("#editTrainingButton").show();
            $("#showNewTrainingTableButton").text("Sakrij");

            $("#trainingName").val(result.Name);
            $("#trainingType").val(result.TrainingType);
            $("#trainingDuration").val(result.Duration);
            $("#trainingCapacity").val(result.VisitorCapacity);
            let year = result.DateOfTraining.slice(0, 4);
            let month = result.DateOfTraining.slice(5, 7);
            let day = result.DateOfTraining.slice(8, 10);
            if (day < 10) {
                day = day.slice(1, 2);  // da prebaci 01 u 1
            }
            let hour = result.DateOfTraining.slice(11, 13);
            let minute = result.DateOfTraining.slice(14, 16);
            $("#trainingYear").val(year);
            $("#trainingMonth").val(months[parseInt(month) - 1]);
            $("#trainingMonth").trigger('change');
            $("#trainingDay").val(day);
            $("#trainingHour").val(hour);
            $("#trainingMinute").val(minute);

            // ucitan korisnik mora da je validan cim je sacuvan, pa su sve vrednosti validne
            isTrainingNameValid = true;
            isTrainingTypeValid = true;
            isTrainingDurationValid = true;
            isTrainingCapacityValid = true;
            isTrainingHourValid = true;
            isTrainingMinuteValid = true;
            isTrainingDateValid = true;
        }).fail(function (data) {
            alert(data.responseJSON);
        });
    }

    $("#trainingName").focusout(function () {
        let name = $("#trainingName").val().trim();
        let nameReg = /^[A-Za-z0-9 ]{3,20}$/;
        let capitalReg = /[A-Z]/
        if (!capitalReg.test(name[0])) {
            isTrainingNameValid = false;
            $("#invalidTrainingName").html("Naziv mora da pocinje velikim pocetnim slovom");
            $("#invalidTrainingName").css("color", "red");
            $("#trainingName").css("border", "1px solid red");
            return;
        }
        if (!nameReg.test(name)) {
            isTrainingNameValid = false;
            let errorMessage = "";
            if (name.length < 3) {
                errorMessage = "Naziv mora da ima bar 3 slova";
            } else if (name.length > 20) {
                errorMessage = "Naziv ne moze da ima vise od 20 slova";
            } else {
                errorMessage = "Naziv ne moze da sadrzi specijalne karaktere";
            }
            $("#invalidTrainingName").html(errorMessage);
            $("#invalidTrainingName").css("color", "red");
            $("#trainingName").css("border", "1px solid red");
            return;
        }
        isTrainingNameValid = true;
        $("#trainingName").css("border", "1px solid black");
        $("#invalidTrainingName").html("");
    });
    
    $("#trainingType").focusout(function () {
        let type = $("#trainingType").val().trim();
        let typeReg = /^[A-Za-z0-9 ]{3,20}$/;
        if (!typeReg.test(type)) {
            isTrainingTypeValid = false;
            let errorMessage = "";
            if (type.length < 3) {
                errorMessage = "Naziv mora da ima bar 3 slova";
            } else if (type.length > 20) {
                errorMessage = "Naziv ne moze da ima vise od 20 slova";
            } else {
                errorMessage = "Naziv ne moze da sadrzi specijalne karaktere";
            }
            $("#invalidTrainingType").html(errorMessage);
            $("#invalidTrainingType").css("color", "red");
            $("#trainingType").css("border", "1px solid red");
            return;
        }
        isTrainingTypeValid = true;
        $("#trainingType").css("border", "1px solid black");
        $("#invalidTrainingType").html("");
    });

    $("#trainingDuration").focusout(function () {
        let duration = $("#trainingDuration").val();
        if (isNaN(duration) || duration == "") {
            isTrainingDurationValid = false;
            $("#invalidTrainingDuration").html("Morate uneti broj");
            $("#invalidTrainingDuration").css("color", "red");
            $("#trainingDuration").css("border", "1px solid red");
            return;
        }
        if (duration < 0 || duration > 999) {
            isTrainingDurationValid = false;
            $("#invalidTrainingDuration").html("Uneti broj mora biti veci od 0 i manji od 1000");
            $("#invalidTrainingDuration").css("color", "red");
            $("#trainingDuration").css("border", "1px solid red");
            return;
        }
        isTrainingDurationValid = true;
        $("#trainingDuration").css("border", "1px solid black");
        $("#invalidTrainingDuration").html("");
    });

    $("#trainingCapacity").focusout(function () {
        let capacity = $("#trainingCapacity").val();
        if (isNaN(capacity) || capacity == "") {
            isTrainingCapacityValid = false;
            $("#invalidTrainingCapacity").html("Morate uneti broj");
            $("#invalidTrainingCapacity").css("color", "red");
            $("#trainingCapacity").css("border", "1px solid red");
            return;
        }
        if (capacity < 0 || capacity > 999) {
            isTrainingCapacityValid = false;
            $("#invalidTrainingCapacity").html("Uneti broj mora biti veci od 0 i manji od 1000");
            $("#invalidTrainingCapacity").css("color", "red");
            $("#trainingCapacity").css("border", "1px solid red");
            return;
        }
        isTrainingCapacityValid = true;
        $("#trainingCapacity").css("border", "1px solid black");
        $("#invalidTrainingCapacity").html("");
    });

    $("#trainingHour").focusout(function () {
        let hour = $("#trainingHour").val();
        if (isNaN(hour) || hour == "") {
            isTrainingHourValid = false;
            $("#trainingHour").css("border", "1px solid red");
            return;
        }
        if (hour < 0 || hour > 23) {
            isTrainingHourValid = false;
            $("#trainingHour").css("border", "1px solid red");
            return;
        }
        isTrainingHourValid = true;
        $("#trainingHour").css("border", "1px solid black");
    });

    $("#trainingMinute").focusout(function () {
        let minute = $("#trainingMinute").val();
        if (isNaN(minute) || minute == "") {
            isTrainingMinuteValid = false;
            $("#trainingMinute").css("border", "1px solid red");
            return;
        }
        if (minute < 0 || minute > 59) {
            isTrainingMinuteValid = false;
            $("#trainingMinute").css("border", "1px solid red");
            return;
        }
        isTrainingMinuteValid = true;
        $("#trainingMinute").css("border", "1px solid black");
    });

    $("#createTrainingButton").click(function () {
        if (!isTrainingNameValid || !isTrainingTypeValid || !isTrainingDurationValid || !isTrainingCapacityValid || !isTrainingHourValid || !isTrainingMinuteValid) {
            $("#trainingName").trigger('focusout');
            $("#trainingType").trigger('focusout');
            $("#trainingDuration").trigger('focusout');
            $("#trainingCapacity").trigger('focusout');
            $("#trainingHour").trigger('focusout');
            $("#trainingMinute").trigger('focusout');
            return;
        }
        let year = $("#trainingYear").val();
        let month = $("#trainingMonth").val();
        let day = $("#trainingDay").val();
        let hour = $("#trainingHour").val();
        let minute = $("#trainingMinute").val();
        let date = GetDateOfTraining(year, month, day, hour, minute);
        isTrainingDateValid = CheckIfDateIsValid(date,3);   // 3 je offset, gledamo 3 dana unapred
        if (!isTrainingDateValid) {
            $("#invalidTrainingDate").html("Trening mora biti bar 3 dana unapred");
            $("#invalidTrainingDate").css("color", "red");
            return;
        }
        $("#invalidTrainingDate").html("");
        let name = $("#trainingName").val().trim();
        let type = $("#trainingType").val().trim();
        let duration = $("#trainingDuration").val();
        let capacity = $("#trainingCapacity").val();
        $.post('/api/grouptrainings', {'name' : name, 'trainingType' : type, 'duration' : duration, 'dateOfTraining' : date.toISOString(), 'visitorCapacity' : capacity},
            function (result) {
                // result.responeJSON.Message je undefined ovde, ali dole moze
                // samo result je string koji posaljemo u Ok("text");
                alert(result);
                $("#showNewTrainingTableButton").trigger('click');
                GenerateGroupTrainingTable();
            }
        ).fail(function (data) {
            alert(data.responseJSON);
        });
    });

    $("#editTrainingButton").click(function () {
        if (!isTrainingNameValid || !isTrainingTypeValid || !isTrainingDurationValid || !isTrainingCapacityValid || !isTrainingHourValid || !isTrainingMinuteValid) {
            return;
        }
        let year = $("#trainingYear").val();
        let month = $("#trainingMonth").val();
        let day = $("#trainingDay").val();
        let hour = $("#trainingHour").val();
        let minute = $("#trainingMinute").val();
        let date = GetDateOfTraining(year, month, day, hour, minute);
        isTrainingDateValid = CheckIfDateIsValid(date, 0);   // 0 je offset, gledamo 0 dana unapred za izmenu
        if (!isTrainingDateValid) {
            $("#invalidTrainingDate").html("Trening ne moze biti u proslosti");
            $("#invalidTrainingDate").css("color", "red");
            return;
        }
        $("#invalidTrainingDate").html("");
        let name = $("#trainingName").val().trim();
        let type = $("#trainingType").val().trim();
        let duration = $("#trainingDuration").val();
        let capacity = $("#trainingCapacity").val();
        $.ajax("/api/grouptrainings", {
            method: 'PUT',
            data: {'id' : groupTrainingId, 'name': name, 'trainingType': type, 'duration': duration, 'dateOfTraining': date.toISOString(), 'visitorCapacity': capacity },
            success: function (result) {
                alert(result);
                $("#showNewTrainingTableButton").trigger('click');
                GenerateGroupTrainingTable();
            }
        }).fail(function (data) {
            alert(data.responseJSON);
        });
    });

    function GetDateOfTraining(year, month, day, hour, minute) {
        if (day < 10) {
            day = "0" + day;
        }
        for (let i in months) {
            if (months[i] == month) {
                month = parseInt(i) + 1;
                if (month < 10) {
                    month = "0" + month;
                }
                break;
            }
        }
        let date = year + "-" + month + "-" + day;
        let retVal = new Date(date);
        retVal.setHours(hour);
        retVal.setMinutes(minute);
        return retVal;
    }

    function CheckIfDateIsValid(date, offset) {
        let today = new Date();
        today.setDate(today.getDate() + offset);

        let day1 = today.getDate();
        let month1 = today.getMonth();
        let year1 = today.getFullYear();
        let hour1 = today.getHours();
        let minute1 = today.getMinutes();

        let day2 = date.getDate();
        let month2 = date.getMonth();
        let year2 = date.getFullYear();
        let hour2 = date.getHours();
        let minute2 = date.getMinutes();

        if (year1 < year2) {
            return true;
        }

        if (year1 > year2) {
            return false;
        }

        if (month1 < month2) {
            return true;
        }

        if (month1 > month2) {
            return false;
        }

        if (day1 < day2) {
            return true;
        }

        if (day1 > day2) {
            return false;
        }

        if (hour1 < hour2) {
            return true;
        }

        if (hour1 > hour2) {
            return false;
        }

        if (minute1 < minute2) {
            return true;
        }

        if (minute1 > minute2) {
            return false;
        }
        return true;
    }

    function EmptyTrainingTableFields() {
        $("#trainingName").val("");
        $("#trainingName").css("border", "1px solid black");
        $("#invalidTrainingName").html("");

        $("#trainingType").val("");
        $("#trainingType").css("border", "1px solid black");
        $("#invalidTrainingType").html("");

        $("#trainingDuration").val("");
        $("#trainingDuration").css("border", "1px solid black");
        $("#invalidTrainingDuration").html("");

        $("#trainingCapacity").val("");
        $("#trainingCapacity").css("border", "1px solid black");
        $("#invalidTrainingCapacity").html("");

        $("#trainingYear").val("2022");
        $("#trainingMonth").val("Januar");
        GetOptionsForDay("2022", "Januar");

        $("#trainingHour").val("00");
        $("#trainingHour").css("border", "1px solid black");

        $("#trainingMinute").val("00");
        $("#trainingMinute").css("border", "1px solid black");

        $("#invalidTrainingDate").html("");

        isTrainingNameValid = false;
        isTrainingTypeValid = false;
        isTrainingDurationValid = false;
        isTrainingCapacityValid = false;
        isTrainingHourValid = true;
        isTrainingMinuteValid = true;
        isTrainingDateValid = false;
    }
})