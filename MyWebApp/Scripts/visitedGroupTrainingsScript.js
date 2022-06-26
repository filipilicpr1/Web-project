$(document).ready(function () {
    var ascendingName = true;
    var ascendingType = true;
    var ascendingDate = true;


    $.get("/api/grouptrainings/visitedtrainings", function (data, status) {
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
    });

    $("#trainingPretragaButton").click(function () {
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
        });
    });

    function GenerateTableContent(data) {
        // svaki red ima naziv,adresu,godinu otvaranja, i dugme(link) koje kada se pritisne ide se na drugu stranicu
        // da bi znali koje dugme smo pritisli u url stavimo id fitnes centra, koji posle izvucemo na drugoj stranici i odradimo get
        let tableContent = "<table id='myTable' border='1'><caption align='center'>Poseceni grupni treninzi</caption><tr><th>Fitnes centar</th><th id='nazivSort'>Naziv</th><th id='vrstaTreningaSort'>Vrsta treninga</th><th>Trajanje</th><th id='datumSort'>Datum</th><th>Kapacitet</th><th>Broj ucesnika</th></tr>";
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
                SortByNameAsc(data);
                ascendingName = false;
                ascendingType = true;
                ascendingDate = true;
            } else {
                SortByNameDesc(data);
                ascendingName = true;
                ascendingType = true;
                ascendingDate = true;
            }
            // mora na kraju da se pozove jer svaka sort funkcija zove GenerateTableContent(), pa ponisti efekat
            $("#nazivSort").css("text-decoration", "underline");   // indikator da je soritrano po ovome
        });
        
        $("#vrstaTreningaSort").click(function () {
            if (ascendingType) {
                SortByTypeAsc(data);
                ascendingType = false;
                ascendingName = true;
                ascendingDate = true;
            } else {
                SortByTypeDesc(data);
                ascendingType = true;
                ascendingName = true;
                ascendingDate = true;
            }
            // mora na kraju da se pozove jer svaka sort funkcija zove GenerateTableContent(), pa ponisti efekat
            $("#vrstaTreningaSort").css("text-decoration", "underline");   // indikator da je soritrano po ovome
        });
        
        $("#datumSort").click(function () {
            if (ascendingDate) {
                SortByDateAsc(data);
                ascendingDate = false;
                ascendingName = true;
                ascendingType = true;
            } else {
                SortByDateDesc(data);
                ascendingDate = true;
                ascendingName = true;
                ascendingType = true;
            }
            // mora na kraju da se pozove jer svaka sort funkcija zove GenerateTableContent(), pa ponisti efekat
            $("#datumSort").css("text-decoration", "underline");   // indikator da je soritrano po ovome
        });
    }

    function SortByNameAsc(data) {
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

    function SortByNameDesc(data) {
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

    function SortByTypeAsc(data) {
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

    function SortByTypeDesc(data) {
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

    function SortByDateAsc(data) {
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

    function SortByDateDesc(data) {
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