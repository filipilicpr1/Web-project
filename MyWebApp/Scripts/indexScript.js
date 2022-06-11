$(document).ready(function () {
    var ascendingName = false;
    var ascendingAddress = true;
    var ascendingYear = true;
    $.get("/api/fitnesscenters", function (data, status) {
        GenerateTableContent(data);
    });

    $("#pretragaButton").click(function () {
        let name = $("#nazivPretraga").val();
        let address = $("#adresaPretraga").val();
        let minYear = $("#minGodinaPretraga").val();
        let maxYear = $("#maxGodinaPretraga").val();
        if (isNaN(minYear)) {
            alert('Morate uneti broj');
            $("#minGodinaPretraga").focus();
            return;
        }

        if (isNaN(maxYear)) {
            alert('Morate uneti broj');
            $("#maxGodinaPretraga").focus();
            return;
        }

        if (name == "") {
            name = "noName";
        }

        if (address == "") {
            address = "noAddress";
        }

        if (minYear == "") {
            minYear = "0";
        }

        if (maxYear == "") {
            maxYear = "10000";
        }

        $.get(`/api/fitnesscenters/${name}/${address}/${minYear}/${maxYear}`, function (data, status) {
            GenerateTableContent(data);
        });
    });


    function GenerateTableContent(data) {
        let tableContent = "<table id='myTable' border='1'><tr><th id='nazivSort'>Naziv</th><th id='adresaSort'>Adresa</th><th id='godinaSort'>Godina otvaranja</th><th></th></tr>";
        for (fitnessCenter in data) {
            tableContent += `<tr><td>${data[fitnessCenter].Name}</td><td>${data[fitnessCenter].Address}</td><td>${data[fitnessCenter].YearCreated}</td><td><a href='fitnessCenterDetails.html?${data[fitnessCenter].Name}'><button class="detailsButton">Detalji</button></a></td></tr>`;
        }
        tableContent += "</table>";
        $("#tableDiv").html(tableContent);
        $("#nazivSort").click(function () {
            if (ascendingName) {
                SortByNameAsc(data);
                ascendingName = false;
                ascendingAddress = true;
                ascendingYear = true;
            } else {
                SortByNameDesc(data);
                ascendingName = true;
                ascendingAddress = true;
                ascendingYear = true;
            }
        });

        $("#adresaSort").click(function () {
            if (ascendingAddress) {
                SortByAddressAsc(data);
                ascendingAddress = false;
                ascendingName = true;
                ascendingYear = true;
            } else {
                SortByAddressDesc(data);
                ascendingAddress = true;
                ascendingName = true;
                ascendingYear = true;
            }
        });

        $("#godinaSort").click(function () {
            if (ascendingYear) {
                SortByYearAsc(data);
                ascendingYear = false;
                ascendingName = true;
                ascendingAddress = true;
            } else {
                SortByYearDesc(data);
                ascendingYear = true;
                ascendingName = true;
                ascendingAddress = true;
            }
        });
    }
    
    function SortByNameAsc(data) {
        let sortedList = []
        for (fitnessCenter in data) {
            sortedList.push(data[fitnessCenter])
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
        for (fitnessCenter in data) {
            sortedList.push(data[fitnessCenter])
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

    function SortByAddressAsc(data) {
        let sortedList = []
        for (fitnessCenter in data) {
            sortedList.push(data[fitnessCenter])
        }
        for (let i = 0; i < sortedList.length - 1; i++) {
            let minInd = i;
            for (let j = i + 1; j < sortedList.length; j++) {
                if (sortedList[j].Address < sortedList[minInd].Address) {
                    minInd = j;
                }
            }
            let temp = sortedList[minInd];
            sortedList[minInd] = sortedList[i];
            sortedList[i] = temp;
        }
        GenerateTableContent(sortedList);
    }

    function SortByAddressDesc(data) {
        let sortedList = []
        for (fitnessCenter in data) {
            sortedList.push(data[fitnessCenter])
        }
        for (let i = 0; i < sortedList.length - 1; i++) {
            let maxInd = i;
            for (let j = i + 1; j < sortedList.length; j++) {
                if (sortedList[j].Address > sortedList[maxInd].Address) {
                    maxInd = j;
                }
            }
            let temp = sortedList[maxInd];
            sortedList[maxInd] = sortedList[i];
            sortedList[i] = temp;
        }
        GenerateTableContent(sortedList);
    }

    function SortByYearAsc(data) {
        let sortedList = []
        for (fitnessCenter in data) {
            sortedList.push(data[fitnessCenter])
        }
        for (let i = 0; i < sortedList.length - 1; i++) {
            let minInd = i;
            for (let j = i + 1; j < sortedList.length; j++) {
                if (sortedList[j].YearCreated < sortedList[minInd].YearCreated) {
                    minInd = j;
                }
            }
            let temp = sortedList[minInd];
            sortedList[minInd] = sortedList[i];
            sortedList[i] = temp;
        }
        GenerateTableContent(sortedList);
    }

    function SortByYearDesc(data) {
        let sortedList = []
        for (fitnessCenter in data) {
            sortedList.push(data[fitnessCenter])
        }
        for (let i = 0; i < sortedList.length - 1; i++) {
            let maxInd = i;
            for (let j = i + 1; j < sortedList.length; j++) {
                if (sortedList[j].YearCreated > sortedList[maxInd].YearCreated) {
                    maxInd = j;
                }
            }
            let temp = sortedList[maxInd];
            sortedList[maxInd] = sortedList[i];
            sortedList[i] = temp;
        }
        GenerateTableContent(sortedList);
    }
})