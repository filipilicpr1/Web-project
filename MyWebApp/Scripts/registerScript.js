$(document).ready(function () {
    var months = ['Januar', 'Februar', 'Mart', 'April', 'Maj', 'Jun', 'Jul', 'Avgust', 'Septembar', 'Oktobar', 'Novembar', 'Decembar'];
    var isEmailValid = false;
    var isUsernameValid = false;
    var isPasswordValid = false;
    var isConfirmPasswordValid = false;
    var isNameValid = false;
    var isLastNameValid = false;
    var isDateValid = false;

    $("#registerEmail").focusout(function () {
        let email = $("#registerEmail").val();
        let emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        if (!emailReg.test(email) || (email.length == 0)) {
            isEmailValid = false;
            $("#invalidEmail").html("Uneta vrednost nije email");
            $("#invalidEmail").css("color", "red");
            $("#registerEmail").css("border", "1px solid red");
            return;
        }
        isEmailValid = true;
        $("#registerEmail").css("border", "1px solid black");
        $("#invalidEmail").html("");
    });

    $("#registerName").focusout(function () {
        let name = $("#registerName").val();
        let nameReg = /^[a-zA-Z]{3,16}$/;
        let capitalReg = /[A-Z]/
        if (name.length < 3) {
            isNameValid = false;
            $("#invalidName").html("Ime mora da ima bar 3 slova");
            $("#invalidName").css("color", "red");
            $("#registerName").css("border", "1px solid red");
            return;
        }
        if (!capitalReg.test(name[0])) {
            isNameValid = false;
            $("#invalidName").html("Ime mora da pocinje velikim pocetnim slovom");
            $("#invalidName").css("color", "red");
            $("#registerName").css("border", "1px solid red");
            return;
        }
        if (!nameReg.test(name)) {
            isNameValid = false;
            $("#invalidName").html("Ime ne moze da ime brojeve ili druge specijalne karaktere");
            $("#invalidName").css("color", "red");
            $("#registerName").css("border", "1px solid red");
            return;
        }
        isNameValid = true;
        $("#registerName").css("border", "1px solid black");
        $("#invalidName").html("");
    });

    $("#registerLastName").focusout(function () {
        let lastName = $("#registerLastName").val();
        let lastNameReg = /^[a-zA-Z]{3,16}$/;
        let capitalReg = /[A-Z]/
        if (lastName.length < 3) {
            isLastNameValid = false;
            $("#invalidLastName").html("Prezime mora da ima bar 3 slova");
            $("#invalidLastName").css("color", "red");
            $("#registerLastName").css("border", "1px solid red");
            return;
        }
        if (!capitalReg.test(lastName[0])) {
            isLastNameValid = false;
            $("#invalidLastName").html("Prezime mora da pocinje velikim pocetnim slovom");
            $("#invalidLastName").css("color", "red");
            $("#registerLastName").css("border", "1px solid red");
            return;
        }
        if (!lastNameReg.test(lastName)) {
            isLastNameValid = false;
            $("#invalidLastName").html("Prezime ne moze da ime brojeve ili druge specijalne karaktere");
            $("#invalidLastName").css("color", "red");
            $("#registerLastName").css("border", "1px solid red");
            return;
        }
        isLastNameValid = true;
        $("#registerLastName").css("border", "1px solid black");
        $("#invalidLastName").html("");
    });

    $("#submitButton").click(function () {
        let pol = $("input[name='radioGender']:checked").val();
        alert(pol);
    });

    InitiateYearMonthDay();

    $("#registerYear").change(function () {
        $("#fontMonth").html("Mesec ");
        $("#registerMonth").show();
        let year = $("#registerYear").val();
        let month = $("#registerMonth").val();
        GenerateOptionsForDay(year, month);
    });

    $("#registerMonth").change(function () {
        $("#fontDay").html("Dan ");
        $("#registerDay").show();
        let year = $("#registerYear").val();
        let month = $("#registerMonth").val();
        GenerateOptionsForDay(year, month);
    });

    function InitiateYearMonthDay() {
        $("#registerMonth").hide();
        $("#registerDay").hide();
        GenerateOptionsForMonth();
        GenerateOptionsForYear();
    }


    function GenerateOptionsForYear() {
        for (let i = 1920; i < 2023; i++) {
            $("#registerYear").append('<option>' + i + '</option>');
        }
    }

    function GenerateOptionsForMonth() {
        for (let i in months) {
            $("#registerMonth").append('<option>' + months[i] + '</option>');
        }
    }

    function GenerateOptionsForDay(year, month) {
        $("#registerDay").empty();
        for (let i = 1; i < 29; i++) {
            $("#registerDay").append('<option>' + i + '</option>');
        }

        if (month == "Februar" && !IsLeapYear(year)) {
            return;
        }

        $("#registerDay").append('<option>29</option>');
        if (month == "Februar" && IsLeapYear(year)) {
            return;
        }

        $("#registerDay").append('<option>30</option>');
        if (month == "April" || month == "Jun" || month == "Septembar" || month == "Novembar") {
            return;
        }
        $("#registerDay").append('<option>31</option>');
    }

    function IsLeapYear(year) {
        if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0)) {
            return true;
        }
        return false;
    }

})