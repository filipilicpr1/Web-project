$(document).ready(function () {
    var months = ['Januar', 'Februar', 'Mart', 'April', 'Maj', 'Jun', 'Jul', 'Avgust', 'Septembar', 'Oktobar', 'Novembar', 'Decembar'];
    var ascendingName = false;
    var ascendingAddress = true;
    var ascendingYear = true;
    // cim se ucita stranica uzme se userId , jer ako je korisnik ulogovan treba odmah da vidi neki sadrzaj
    // ako korisnik nije bio ulogovan, pa se uloguje, ovo se ne azurira samo jer se ne refresuje stranica(za sad), pa mora da se opet ucita u funkciji
    var userId = sessionStorage.getItem("userId"); 


    // prikaz odredjenog sadrzaja u zavisnosti od toga da li je korsnik ulogovan ili ne
    if (userId == null || userId == "") {
        // ako nije ulogovan
        // prikazujemo dugme za login, a krijemo dugme za logout
        $("#logoutButton").hide();
        $("#showLoginTableButton").show();

        // prikazujemo dugme za registraciju, krijemo dugme za izmenu
        $("#showEditTable").hide();
        $("#showRegisterTableButton").show();

        // prikazujemo submit za registraciju, krijemo submit za izmenu
        $("#editButton").hide();
        $("#submitButton").show();
    } else {
        // jeste ulogovan
        //prikazujemo dugme za logout, krijemo login
        $("#logoutButton").show();
        $("#showLoginTableButton").hide();

        // prikazujemo dugme za izmenu, krijemo dugme za registraciju
        $("#showRegisterTableButton").hide();
        $("#showEditTable").show();

        // prikazujemo dugme za submit izmene, krijemo submit za registraciju
        $("#editButton").show();
        $("#submitButton").hide();

        // ako je ulogovan, u zavisnosti od uloge, treba da se prikaze dodatan sadrzaj
    }
    // polja(forma) za korisnikove podatke su uvek sakrivena, bez obzira na userId
    $("#registerTable").hide();
    $("#loginTable").hide();

    // popunjavanje combo boxa za godinu i mesec
    InitiateYearMonth();

    function InitiateYearMonth() {
        $("#registerMonth").hide();
        $("#registerDay").hide();
        GenerateOptionsForMonth();
        GenerateOptionsForYear();
    }

    function GenerateOptionsForYear() {
        for (let i = 2022; i > 1920; i--) {
            $("#registerYear").append('<option>' + i + '</option>');
        }
    }

    function GenerateOptionsForMonth() {
        for (let i in months) {
            $("#registerMonth").append('<option>' + months[i] + '</option>');
        }
    }


    // inicijalno popunjavanje tabele sa fitnes centrima pri ucitavanju stranice
    $.get("/api/fitnesscenters", function (data, status) {
        GenerateTableContent(data);
    });


    // event handler kada se pritisne na dugme za pretragu
    // radi validaciju samo za unete godine, i to preko alert, izmeni da se crveni border
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

    // izgenerise tabelu sa fitnes centrima, prosledi se data koji je dobijen sa back-a
    // koristi se i za inicijalno popunjavanje i za pretragu
    function GenerateTableContent(data) {
        // svaki red ima naziv,adresu,godinu otvaranja, i dugme(link) koje kada se pritisne ide se na drugu stranicu
        // da bi znali koje dugme smo pritisli u url stavimo id fitnes centra, koji posle izvucemo na drugoj stranici i odradimo get
        let tableContent = "<table id='myTable' border='1'><tr><th id='nazivSort'>Naziv</th><th id='adresaSort'>Adresa</th><th id='godinaSort'>Godina otvaranja</th><th></th></tr>";
        for (fitnessCenter in data) {
            tableContent += `<tr><td>${data[fitnessCenter].Name}</td><td>${data[fitnessCenter].Address}</td><td>${data[fitnessCenter].YearCreated}</td><td><a href='fitnessCenterDetails.html?${data[fitnessCenter].Id}'><button class="detailsButton">Detalji</button></a></td></tr>`;
        }
        tableContent += "</table>";
        $("#tableDiv").html(tableContent);

        // deklaracije event handlera kada se pritisne na odredjene header-e u tabeli, jer se to koristi za sort
        // fukncije za sortiranje pozivaju GenerateTableContent kome proslede sortiranu listu
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


    // funkcije za sortiranje tabele sa fitnes centrima
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