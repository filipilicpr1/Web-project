$(document).ready(function () {
    var months = ['Januar', 'Februar', 'Mart', 'April', 'Maj', 'Jun', 'Jul', 'Avgust', 'Septembar', 'Oktobar', 'Novembar', 'Decembar'];
    var isEmailValid = false;
    var isUsernameValid = false;
    var isPasswordValid = false;
    var isConfirmPasswordValid = false;
    var isNameValid = false;
    var isLastNameValid = false;
    var isDateValid = false;

    $("#showRegisterTableButton").click(function () {
        if ($("#showRegisterTableButton").text() == "Sakrij") {
            $("#registerTable").hide();
            $("#showRegisterTableButton").text("Registracija novog korisnika");
            return;
        }
        $("#registerTable").show();
        EmptyRegisterFields();
        $("#showRegisterTableButton").text("Sakrij");
    });

    $("#showEditTable").click(function () {
        if ($("#showEditTable").text() == "Sakrij") {
            $("#registerTable").hide();
            $("#showEditTable").text("Izmeni profil");
            return;
        }
        $("#registerTable").show();
        EmptyRegisterFields();
        GenerateProfileFields();
        $("#showEditTable").text("Sakrij");
    });

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
        if (!capitalReg.test(name[0])) {
            isNameValid = false;
            $("#invalidName").html("Ime mora da pocinje velikim pocetnim slovom");
            $("#invalidName").css("color", "red");
            $("#registerName").css("border", "1px solid red");
            return;
        }
        if (!nameReg.test(name)) {
            isNameValid = false;
            let errorMessage = "";
            if (name.length < 3) {
                errorMessage = "Ime mora da ima bar 3 slova";
            } else if (name.length > 16) {
                errorMessage = "Ime ne moze da ima vise od 16 slova";
            } else {
                errorMessage = "Ime ne moze da ime brojeve ili druge specijalne karaktere";
            }
            $("#invalidName").html(errorMessage);
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
        let lastNameReg = /^[a-zA-Z]{3,20}$/;
        let capitalReg = /[A-Z]/;
        if (!capitalReg.test(lastName[0])) {
            isLastNameValid = false;
            $("#invalidLastName").html("Prezime mora da pocinje velikim pocetnim slovom");
            $("#invalidLastName").css("color", "red");
            $("#registerLastName").css("border", "1px solid red");
            return;
        }
        if (!lastNameReg.test(lastName)) {
            isLastNameValid = false;
            let errorMessage = "";
            if (lastName.length < 3) {
                errorMessage = "Prezime mora da ima bar 3 slova";
            } else if (lastName.length > 20) {
                errorMessage = "Prezime ne moze da ima vise od 20 slova";
            } else {
                errorMessage = "Prezime ne moze da ime brojeve ili druge specijalne karaktere";
            }
            $("#invalidLastName").html(errorMessage);
            $("#invalidLastName").css("color", "red");
            $("#registerLastName").css("border", "1px solid red");
            return;
        }
        isLastNameValid = true;
        $("#registerLastName").css("border", "1px solid black");
        $("#invalidLastName").html("");
    });

    $("#registerUsername").focusout(function () {
        let username = $("#registerUsername").val();
        let usernameReg = /^[\w-\.]{3,16}$/;
        if (!usernameReg.test(username)) {
            isUsernameValid = false;
            let errorMessage = "";
            if (username.length < 3) {
                errorMessage = "Korisnicko ime mora da ima bar 3 karaktera";
            } else if (username.length > 16) {
                errorMessage = "Korisnicko ime ne moze da ima vise od 16 karaktera";
            } else {
                errorMessage = "Korisnicko ime moze da ima samo slova,brojeve, donju crtu(_), crtu(-) i tacku(.)";
            }
            $("#invalidUsername").html(errorMessage);
            $("#invalidUsername").css("color", "red");
            $("#registerUsername").css("border", "1px solid red");
            return;
        }
        isUsernameValid = true;
        $("#registerUsername").css("border", "1px solid black");
        $("#invalidUsername").html("");
    });

    $("#registerPassword").focusout(function () {
        let password = $("#registerPassword").val();
        let passwordReg = /^[\w-\.]{3,16}$/;
        if (!passwordReg.test(password)) {
            isPasswordValid = false;
            let errorMessage = "";
            if (password.length < 3) {
                errorMessage = "Lozinka mora da ima bar 3 karaktera";
            } else if (password.length > 16) {
                errorMessage = "Lozinka ne moze da ima vise od 16 karaktera";
            } else {
                errorMessage = "Lozinka moze da ima samo slova,brojeve, donju crtu(_), crtu(-) i tacku(.)";
            }
            $("#invalidPassword").html(errorMessage);
            $("#invalidPassword").css("color", "red");
            $("#registerPassword").css("border", "1px solid red");
            return;
        }
        isPasswordValid = true;
        $("#registerPassword").css("border", "1px solid black");
        $("#invalidPassword").html("");
        let confirmPassword = $("#registerConfirmPassword").val();
        if (confirmPassword.length != 0) {
            $("#registerConfirmPassword").trigger('focusout');
        }
    });

    $("#registerConfirmPassword").focusout(function () {
        let password = $("#registerPassword").val();
        let confirmPassword = $("#registerConfirmPassword").val();
        if (password != confirmPassword || confirmPassword.length == 0) {
            isConfirmPasswordValid = false;
            $("#invalidConfirmPassword").html("Lozinke se moraju poklapati");
            $("#invalidConfirmPassword").css("color", "red");
            $("#registerConfirmPassword").css("border", "1px solid red");
            return;
        }
        isConfirmPasswordValid = true;
        $("#registerConfirmPassword").css("border", "1px solid black");
        $("#invalidConfirmPassword").html("");
    });

    $("#submitButton").click(function () {
        if (!isUsernameValid || !isPasswordValid || !isConfirmPasswordValid || !isEmailValid || !isNameValid || !isLastNameValid || !isDateValid) {
            $("#registerUsername").trigger('focusout');
            $("#registerPassword").trigger('focusout');
            $("#registerConfirmPassword").trigger('focusout');
            $("#registerEmail").trigger('focusout');
            $("#registerName").trigger('focusout');
            $("#registerLastName").trigger('focusout');
            if (!isDateValid) {
                $("#invalidDate").html("Morate uneti datum rodjenja");
                $("#invalidDate").css("color", "red");
            }
            return;
        }
        let username = $("#registerUsername").val();
        let password = $("#registerPassword").val();
        let email = $("#registerEmail").val();
        let name = $("#registerName").val();
        let lastName = $("#registerLastName").val();
        let gender = $("input[name='radioGender']:checked").val();
        let day = $("#registerDay").val();
        if (day < 10) {
            day = "0" + day;
        }
        let month;
        for (let i in months) {
            if (months[i] == $("#registerMonth").val()) {
                month = parseInt(i) + 1;
                if (month < 10) {
                    month = "0" + month;
                }
                break;
            }
        }
        let date = $("#registerYear").val() + "-" + month + "-" + day;
        let birthDate = new Date(date);
        $.post('/api/users', { 'username': username, 'password': password, 'name': name, 'lastName': lastName, 'gender': gender, 'email': email, 'birthDate': birthDate.toISOString() },
            function (result) {
                alert(result);
                $("#showRegisterTableButton").trigger('click');
            }
        ).fail(function (data) {
            alert(data.responseJSON.Message);
        });
    });

    $("#editButton").click(function () {
        let username = $("#registerUsername").val();
        let password = $("#registerPassword").val();
        let email = $("#registerEmail").val();
        let name = $("#registerName").val();
        let lastName = $("#registerLastName").val();
        let gender = $("input[name='radioGender']:checked").val();
        let day = $("#registerDay").val();
        if (day < 10) {
            day = "0" + day;
        }
        let month;
        for (let i in months) {
            if (months[i] == $("#registerMonth").val()) {
                month = parseInt(i) + 1;
                if (month < 10) {
                    month = "0" + month;
                }
                break;
            }
        }
        let date = $("#registerYear").val() + "-" + month + "-" + day;
        let birthDate = new Date(date);
        let id = sessionStorage.getItem("userId");
        $.ajax("/api/users/", {
            method: 'PUT',
            data: {'id' : id, 'username': username, 'password': password, 'name': name, 'lastName': lastName, 'gender': gender, 'email': email, 'birthDate': birthDate.toISOString() },
            success: function (result) {
                alert(result);
                $("#showEditTable").trigger('click');
            }
        }).fail(function (data) {
            alert(data.responseJSON.Message);
        });
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
        isDateValid = true;
        $("#invalidDate").html("");
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
        for (let i = 2022; i > 1920; i--) {
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

    function EmptyRegisterFields() {
        $("#registerUsername").val("");
        $("#registerUsername").css("border", "1px solid black");
        $("#invalidUsername").html("");

        $("#registerPassword").val("");
        $("#registerPassword").css("border", "1px solid black");
        $("#invalidPassword").html("");

        $("#registerConfirmPassword").val("");
        $("#registerConfirmPassword").css("border", "1px solid black");
        $("#invalidConfirmPassword").html("");

        $("#registerEmail").val("");
        $("#registerEmail").css("border", "1px solid black");
        $("#invalidEmail").html("");

        $("#registerName").val("");
        $("#registerName").css("border", "1px solid black");
        $("#invalidName").html("");

        $("#registerLastName").val("");
        $("#registerLastName").css("border", "1px solid black");
        $("#invalidLastName").html("");

        $("#registerYear").val("2022");
        $("#registerMonth").val("Januar");
        $("#registerMonth").hide();
        $("#registerDay").hide();
        $("#fontDay").html("");
        $("#fontMonth").html("");
        $("#invalidDate").html("");
        $("input[name='radioGender'][value='Muski']").prop("checked", true);
        isEmailValid = false;
        isUsernameValid = false;
        isPasswordValid = false;
        isConfirmPasswordValid = false;
        isNameValid = false;
        isLastNameValid = false;
        isDateValid = false;
    }

    function GenerateProfileFields() {
        id = sessionStorage.getItem("userId");
        $.get('/api/users', { 'id': id }, function (result) {
            $("#registerUsername").val(result.Username);
            $("#registerPassword").val(result.Password);
            $("#registerConfirmPassword").val(result.Password);
            $("#registerEmail").val(result.Email);
            $("input[name='radioGender'][value=" + result.Gender + "]").prop("checked", true);
            $("#registerName").val(result.Name);
            $("#registerLastName").val(result.LastName);
            //1998-12-18
            let year = result.BirthDate.slice(0, 4);
            let month = result.BirthDate.slice(5, 7);
            let day = result.BirthDate.slice(8, 10);
            if (day < 10) {
                day = day.slice(1, 2);
            }
            $("#registerYear").val(year);
            $("#registerMonth").val(months[parseInt(month) - 1]);
            $("#registerYear").trigger('change');
            $("#registerMonth").trigger('change');
            $("#registerDay").val(day);
        });
    }
})