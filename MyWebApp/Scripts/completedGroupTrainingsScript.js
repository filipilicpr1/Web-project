$(document).ready(function () {
    var months = ['Januar', 'Februar', 'Mart', 'April', 'Maj', 'Jun', 'Jul', 'Avgust', 'Septembar', 'Oktobar', 'Novembar', 'Decembar'];
    var ascendingName = true;
    var ascendingType = true;
    var ascendingDate = true;
    var userId = sessionStorage.getItem("userId");
    var userType = ["POSETILAC", "TRENER", "VLASNIK"];
    var userIsVisitor = false;
    var userIsTrainer = false;

    InitiateYearMonthDayHourMin();
    $("#trainingTableDiv").html("<h1>Nema pravo da pristupite ovom sadrzaju</h1>");
    $("#trainingPretraga").hide();

    function InitiateYearMonthDayHourMin() {
        GenerateOptionsForMonthTrainer();
        GenerateOptionsForYearTrainer();
        let minYear = $("#minYear").val();
        let maxYear = $("#maxYear").val();
        let month = $("#minMonth").val();
        GenerateOptionsForDayTrainer(minYear, month, "#minDay");
        GenerateOptionsForDayTrainer(maxYear, month, "#maxDay");
        // danasnji dan za max datum
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth();
        var yyyy = today.getFullYear();
        $("#maxYear").val(yyyy);
        $("#maxMonth").val(months[mm]);
        $("#maxDay").val(dd);
        $("#minHour").val("00");
        $("#minMin").val("00");
        $("#maxHour").val("23");
        $("#maxMin").val("59");
    }

    function GenerateOptionsForYearTrainer() {
        for (let i = 2022; i > 1980; i--) {
            $("#minYear").append('<option>' + i + '</option>');
            $("#maxYear").append('<option>' + i + '</option>');
        }
        $("#minYear").val("1981");
        $("#maxYear").val("2022");
    }

    function GenerateOptionsForMonthTrainer() {
        for (let i in months) {
            $("#minMonth").append('<option>' + months[i] + '</option>');
            $("#maxMonth").append('<option>' + months[i] + '</option>');
        }
    }

    function GenerateOptionsForDayTrainer(year, month, selector) {
        $(selector).empty();
        for (let i = 1; i < 29; i++) {
            $(selector).append('<option>' + i + '</option>');
        }

        if (month == "Februar" && !IsLeapYear(year)) {
            return;
        }

        $(selector).append('<option>29</option>');
        if (month == "Februar" && IsLeapYear(year)) {
            return;
        }

        $(selector).append('<option>30</option>');
        if (month == "April" || month == "Jun" || month == "Septembar" || month == "Novembar") {
            return;
        }
        $(selector).append('<option>31</option>');
    }

    function IsLeapYear(year) {
        if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0)) {
            return true;
        }
        return false;
    }

    $("#minYear").change(function () {
        let year = $("#minYear").val();
        let month = $("#minMonth").val();
        let day = $("#minDay").val();
        // ovo resetuje dan, pa proverimo da li novi select sadrzi stari dan
        GenerateOptionsForDayTrainer(year, month, "#minDay");
        var exists = false;
        $('#minDay  option').each(function () {
            if (this.value == day) {
                exists = true;
            }
        });
        if (exists) {
            $("#minDay").val(day);
        }
    });

    $("#maxYear").change(function () {
        let year = $("#maxYear").val();
        let month = $("#maxMonth").val();
        let day = $("#maxDay").val();
        // ovo resetuje dan, pa proverimo da li novi select sadrzi stari dan
        GenerateOptionsForDayTrainer(year, month, "#maxDay");
        var exists = false;
        $('#maxDay  option').each(function () {
            if (this.value == day) {
                exists = true;
            }
        });
        if (exists) {
            $("#maxDay").val(day);
        }
    });

    $("#minMonth").change(function () {
        let year = $("#minYear").val();
        let month = $("#minMonth").val();
        let day = $("#minDay").val();
        // ovo resetuje dan, pa proverimo da li novi select sadrzi stari dan
        GenerateOptionsForDayTrainer(year, month, "#minDay");
        var exists = false;
        $('#minDay  option').each(function () {
            if (this.value == day) {
                exists = true;
            }
        });
        if (exists) {
            $("#minDay").val(day);
        }
    });

    $("#maxMonth").change(function () {
        let year = $("#maxYear").val();
        let month = $("#maxMonth").val();
        let day = $("#maxDay").val();
        // ovo resetuje dan, pa proverimo da li novi select sadrzi stari dan
        GenerateOptionsForDayTrainer(year, month, "#maxDay");
        var exists = false;
        $('#maxDay  option').each(function () {
            if (this.value == day) {
                exists = true;
            }
        });
        if (exists) {
            $("#maxDay").val(day);
        }
    });

    GetTrainingUser();

    function GetTrainingUser() {
        if (userId != null && userId != "") {
            $.get("/api/users", { 'id': userId }, function (data, status) {
                // dobavimo usera, pa odredimo koji je tip 
                userIsVisitor = userType[data.UserType] == "POSETILAC"; // za sad se generise samo sadrzaj za posetioca
                userIsTrainer = userType[data.UserType] == "TRENER";
                // u zavisnosti od tipa prikazemo odredjene dugmice
                let path = "";
                if (userIsVisitor) {
                    path = "/api/grouptrainings/visitedtrainings";
                    $("#visitorPretragaButton").show();
                    $("#trainerPretragaButton").hide();
                    $("#visitorSearch").show();
                    $("#trainerMinSearch").hide();
                    $("#trainerMaxSearch").hide();
                    $("#trainingPretraga").show();
                    $("#trainingTableDiv").html("");
                }

                if (userIsTrainer) {
                    path = "/api/grouptrainings/completedtrainings";
                    $("#visitorPretragaButton").hide();
                    $("#trainerPretragaButton").show();
                    $("#visitorSearch").hide();
                    $("#trainerMinSearch").show();
                    $("#trainerMaxSearch").show();
                    $("#trainingPretraga").show();
                    $("#trainingTableDiv").html("");
                }
                if (path == "") {
                    return;
                }
                GetData(path);
            }).fail(function (data) {
                alert(data.responseJSON);
            });
        }
    }

    function GetData(path) {
        $.get(path, function (data, status) {
            // ako je data null onda korisnik nije autorizovan
            if (data == null) {
                $("#trainingTableDiv").html("<h1>Nema pravo da pristupite ovom sadrzaju</h1>");
                $("#trainingPretraga").hide();
                return;
            }

            if (data.length == 0) {
                $("#trainingTableDiv").html("<h1>Jos uvek niste bili ni na jednom treningu</h1>");
                $("#trainingPretraga").hide();
                return;
            }
            GenerateTableContent(data);
        }).fail(function (data) {
            alert(data.responseJSON);
        });
    }
    
    $("#visitorPretragaButton").click(function () {
        let name = $("#nazivPretraga").val();
        let trainingType = $("#tipPretraga").val();
        let fitnessCenter = $("#fitnesCentarPretraga").val();


        if (name == "") {
            name = "noName";
        }

        if (trainingType == "") {
            trainingType = "noType";
        }

        if (fitnessCenter == "") {
            fitnessCenter = "noFitnessCenter";
        }

        $.get(`/api/grouptrainings/${fitnessCenter}/${trainingType}/${name}`, function (data, status) {
            // ako je data null onda korisnik nije autorizovan, npr unese link u search bar
            if (data == null) {
                $("#trainingTableDiv").html("<h1>Nema pravo da pristupite ovom sadrzaju</h1>");
                $("#trainingPretraga").hide();
                return;
            }
            GenerateTableContent(data);
        }).fail(function (data) {
            alert(data.responseJSON);
        });
    });

    $("#trainerPretragaButton").click(function () {
        let minHour = $("#minHour").val() == "" ? "00" : $("#minHour").val();
        let maxHour = $("#maxHour").val() == "" ? "23" : $("#maxHour").val();
        let minMin = $("#minMin").val() == "" ? "00" : $("#minMin").val();
        let maxMin = $("#maxMin").val == "" ? "59" : $("#maxMin").val();
        
        if (!ValidateHourMinute(minHour,maxHour,minMin,maxMin)) {
            return;
        }

        let minYear = $("#minYear").val();
        let minMonth = $("#minMonth").val();
        let minDay = $("#minDay").val();
        let maxYear = $("#maxYear").val();
        let maxMonth = $("#maxMonth").val();
        let maxDay = $("#maxDay").val();
        let minDate = GetTrainingDate(minYear, minMonth, minDay, minHour, minMin);
        let maxDate = GetTrainingDate(maxYear, maxMonth, maxDay, maxHour, maxMin);

        let name = $("#nazivPretraga").val();
        let trainingType = $("#tipPretraga").val();
        if (name == "") {
            name = "noName";
        }

        if (trainingType == "") {
            trainingType = "noType";
        }

        $.get(`/api/grouptrainings/trainersearch`, {'name' : name, 'trainingType' : trainingType, 'minDate' :minDate.toISOString(), 'maxDate' : maxDate.toISOString()} ,function (data, status) {
            // ako je data null onda korisnik nije autorizovan, npr unese link u search bar
            if (data == null) {
                $("#trainingTableDiv").html("<h1>Nema pravo da pristupite ovom sadrzaju</h1>");
                $("#trainingPretraga").hide();
                return;
            }
            GenerateTableContent(data);
        }).fail(function (data) {
            alert(data.responseJSON);
        });
    });

    function ValidateHourMinute(minHour, maxHour, minMin, maxMin) {
        let isValid = true;
        $("#minHour").css("border", "1px solid black");
        $("#maxHour").css("border", "1px solid black");
        $("#minMin").css("border", "1px solid black");
        $("#maxMin").css("border", "1px solid black");

        if (isNaN(minHour)) {
            $("#minHour").css("border", "1px solid red");
            isValid = false;
        }

        if (isNaN(maxHour)) {
            $("#maxHour").css("border", "1px solid red");
            isValid = false;
        }

        if (isNaN(minMin)) {
            $("#minMin").css("border", "1px solid red");
            isValid = false;
        }

        if (isNaN(maxMin)) {
            $("#maxMin").css("border", "1px solid red");
            isValid = false;
        }

        if (minHour < 0 || minHour > 23) {
            $("#minHour").css("border", "1px solid red");
            isValid = false;
        }

        if (maxHour < 0 || maxHour > 23) {
            $("#maxHour").css("border", "1px solid red");
            isValid = false;
        }

        if (minMin < 0 || minMin > 59) {
            $("#minMin").css("border", "1px solid red");
            isValid = false;
        }

        if (maxMin < 0 || maxMin > 59) {
            $("#maxMin").css("border", "1px solid red");
            isValid = false;
        }
        return isValid;
    }

    function GetTrainingDate(year, month, day, hour, minute) {
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

    function GenerateTableContent(data) {
        // svaki red ima naziv,adresu,godinu otvaranja, i dugme(link) koje kada se pritisne ide se na drugu stranicu
        // da bi znali koje dugme smo pritisli u url stavimo id fitnes centra, koji posle izvucemo na drugoj stranici i odradimo get
        let tableContent = "<table id='myTable' border='1'><caption align='center'>Odradjeni grupni treninzi</caption><tr><th>Fitnes centar</th><th id='nazivSort'>Naziv</th><th id='vrstaTreningaSort'>Vrsta treninga</th><th>Trajanje</th><th id='datumSort'>Datum</th><th>Kapacitet</th><th>Broj ucesnika</th></tr>";
        for (groupTraining in data) {
            let datum = data[groupTraining].DateOfTraining.slice(8, 10) + "/" + data[groupTraining].DateOfTraining.slice(5, 7) + "/" + data[groupTraining].DateOfTraining.slice(0, 4);// 2022-10-10
            let vreme = data[groupTraining].DateOfTraining.slice(11, 16);
            let date = datum + " " + vreme;
            tableContent += `<tr><td>${data[groupTraining].FitnessCenterLocation.Name}</td><td>${data[groupTraining].Name}</td><td>${data[groupTraining].TrainingType}</td><td>${data[groupTraining].Duration}</td><td>${date}</td><td>${data[groupTraining].VisitorCapacity}</td><td>${data[groupTraining].VisitorCount}</td></tr>`;
        }
        tableContent += "</table>";
        $("#trainingTableDiv").html(tableContent);
        
        // deklaracije event handlera kada se pritisne na odredjene header-e u tabeli, jer se to koristi za sort
        // fukncije za sortiranje pozivaju GenerateTableContent kome proslede sortiranu listu
        $("#nazivSort").click(function () {
            if (ascendingName) {
                SortTrainingsByNameAsc(data);
                ascendingName = false;
                ascendingType = true;
                ascendingDate = true;
            } else {
                SortTrainingsByNameDesc(data);
                ascendingName = true;
                ascendingType = true;
                ascendingDate = true;
            }
            // mora na kraju da se pozove jer svaka sort funkcija zove GenerateTableContent(), pa ponisti efekat
            $("#nazivSort").css("text-decoration", "underline");   // indikator da je soritrano po ovome
        });
        
        $("#vrstaTreningaSort").click(function () {
            if (ascendingType) {
                SortTrainingsByTypeAsc(data);
                ascendingType = false;
                ascendingName = true;
                ascendingDate = true;
            } else {
                SortTrainingsByTypeDesc(data);
                ascendingType = true;
                ascendingName = true;
                ascendingDate = true;
            }
            // mora na kraju da se pozove jer svaka sort funkcija zove GenerateTableContent(), pa ponisti efekat
            $("#vrstaTreningaSort").css("text-decoration", "underline");   // indikator da je soritrano po ovome
        });
        
        $("#datumSort").click(function () {
            if (ascendingDate) {
                SortTrainingsByDateAsc(data);
                ascendingDate = false;
                ascendingName = true;
                ascendingType = true;
            } else {
                SortTrainingsByDateDesc(data);
                ascendingDate = true;
                ascendingName = true;
                ascendingType = true;
            }
            // mora na kraju da se pozove jer svaka sort funkcija zove GenerateTableContent(), pa ponisti efekat
            $("#datumSort").css("text-decoration", "underline");   // indikator da je soritrano po ovome
        });
    }

    function SortTrainingsByNameAsc(data) {
        let sortedList = []
        for (groupTraining in data) {
            sortedList.push(data[groupTraining])
        }
        for (let i = 0; i < sortedList.length - 1; i++) {
            let minInd = i;
            for (let j = i + 1; j < sortedList.length; j++) {
                if (sortedList[j].Name < sortedList[minInd].Name) {
                    minInd = j;
                }
            }
            let temp = sortedList[minInd];
            sortedList[minInd] = sortedList[i];
            sortedList[i] = temp;
        }
        GenerateTableContent(sortedList);
    }

    function SortTrainingsByNameDesc(data) {
        let sortedList = []
        for (groupTraining in data) {
            sortedList.push(data[groupTraining])
        }
        for (let i = 0; i < sortedList.length - 1; i++) {
            let maxInd = i;
            for (let j = i + 1; j < sortedList.length; j++) {
                if (sortedList[j].Name > sortedList[maxInd].Name) {
                    maxInd = j;
                }
            }
            let temp = sortedList[maxInd];
            sortedList[maxInd] = sortedList[i];
            sortedList[i] = temp;
        }
        GenerateTableContent(sortedList);
    }

    function SortTrainingsByTypeAsc(data) {
        let sortedList = []
        for (groupTraining in data) {
            sortedList.push(data[groupTraining])
        }
        for (let i = 0; i < sortedList.length - 1; i++) {
            let minInd = i;
            for (let j = i + 1; j < sortedList.length; j++) {
                if (sortedList[j].TrainingType < sortedList[minInd].TrainingType) {
                    minInd = j;
                }
            }
            let temp = sortedList[minInd];
            sortedList[minInd] = sortedList[i];
            sortedList[i] = temp;
        }
        GenerateTableContent(sortedList);
    }

    function SortTrainingsByTypeDesc(data) {
        let sortedList = []
        for (groupTraining in data) {
            sortedList.push(data[groupTraining])
        }
        for (let i = 0; i < sortedList.length - 1; i++) {
            let maxInd = i;
            for (let j = i + 1; j < sortedList.length; j++) {
                if (sortedList[j].TrainingType > sortedList[maxInd].TrainingType) {
                    maxInd = j;
                }
            }
            let temp = sortedList[maxInd];
            sortedList[maxInd] = sortedList[i];
            sortedList[i] = temp;
        }
        GenerateTableContent(sortedList);
    }

    function SortTrainingsByDateAsc(data) {
        let sortedList = []
        let dateList = []
        for (groupTraining in data) {
            let datum = data[groupTraining].DateOfTraining.slice(8, 10) + "/" + data[groupTraining].DateOfTraining.slice(5, 7) + "/" + data[groupTraining].DateOfTraining.slice(0, 4);// 2022-10-10
            let vreme = data[groupTraining].DateOfTraining.slice(11, 16);
            let date = datum + " " + vreme;
            sortedList.push(data[groupTraining])
            dateList.push(date);
        }
        for (let i = 0; i < dateList.length - 1; i++) {
            let minInd = i;
            for (let j = i + 1; j < dateList.length; j++) {
                if (CompareDates(dateList[j],dateList[minInd])) {
                    minInd = j;
                }
            }
            let temp = sortedList[minInd];
            sortedList[minInd] = sortedList[i];
            sortedList[i] = temp;
            let dateTemp = dateList[minInd];
            dateList[minInd] = dateList[i];
            dateList[i] = dateTemp;
        }
        GenerateTableContent(sortedList);
    }

    function SortTrainingsByDateDesc(data) {
        let sortedList = []
        let dateList = []
        for (groupTraining in data) {
            let datum = data[groupTraining].DateOfTraining.slice(8, 10) + "/" + data[groupTraining].DateOfTraining.slice(5, 7) + "/" + data[groupTraining].DateOfTraining.slice(0, 4);// 2022-10-10
            let vreme = data[groupTraining].DateOfTraining.slice(11, 16);
            let date = datum + " " + vreme;
            sortedList.push(data[groupTraining])
            dateList.push(date);
        }
        for (let i = 0; i < dateList.length - 1; i++) {
            let maxInd = i;
            for (let j = i + 1; j < dateList.length; j++) {
                if (!CompareDates(dateList[j], dateList[maxInd])) {
                    maxInd = j;
                }
            }
            let temp = sortedList[maxInd];
            sortedList[maxInd] = sortedList[i];
            sortedList[i] = temp;
            let dateTemp = dateList[maxInd];
            dateList[maxInd] = dateList[i];
            dateList[i] = dateTemp;
        }
        GenerateTableContent(sortedList);
    }

    // vraca true ake je date1 pre date2
    function CompareDates(date1, date2) {
        let day1 = date1.slice(0, 2);
        let month1 = date1.slice(3, 5);
        let year1 = date1.slice(6, 10);
        let hour1 = date1.slice(11, 13);
        let minute1 = date1.slice(14, 16);

        let day2 = date2.slice(0, 2);
        let month2 = date2.slice(3, 5);
        let year2 = date2.slice(6, 10);
        let hour2 = date2.slice(11, 13);
        let minute2 = date2.slice(14, 16);

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
        return false;
    }
})