$(document).ready(function () {
    var isLoginUsernameValid = false;
    var isLoginPasswordValid = false;


    $("#showLoginTableButton").click(function () {
        if ($("#showLoginTableButton").text() == "Sakrij") {
            $("#loginTable").hide();
            $("#showLoginTableButton").text("Prijava korisnika");
            return;
        }
        $("#loginTable").show();
        EmptyLoginFields();
        $("#showLoginTableButton").text("Sakrij");
    });
    
    $("#loginUsername").focusout(function () {
        let username = $("#loginUsername").val();
        let usernameReg = /^[\w-\.]{3,16}$/;
        if (!usernameReg.test(username)) {
            isLoginUsernameValid = false;
            $("#invalidLoginUsername").html("Nevalidno korisnicko ime");
            $("#invalidLoginUsername").css("color", "red");
            $("#loginUsername").css("border", "1px solid red");
            return;
        }
        isLoginUsernameValid = true;
        $("#loginUsername").css("border", "1px solid black");
        $("#invalidLoginUsername").html("");
    });

    $("#loginPassword").focusout(function () {
        let password = $("#loginPassword").val();
        let passwordReg = /^[\w-\.]{3,16}$/;
        if (!passwordReg.test(password)) {
            isLoginPasswordValid = false;
            $("#invalidLoginPassword").html("Nevalidna lozinka");
            $("#invalidLoginPassword").css("color", "red");
            $("#loginPassword").css("border", "1px solid red");
            return;
        }
        isLoginPasswordValid = true;
        $("#loginPassword").css("border", "1px solid black");
        $("#invalidLoginPassword").html("");
    });

    $("#loginButton").click(function () {
        if (!isLoginPasswordValid || !isLoginUsernameValid) {
            $("#loginUsername").trigger('focusout');
            $("#loginPassword").trigger('focusout');
            return;
        }
        let username = $("#loginUsername").val();
        let password = $("#loginPassword").val();
        $.get('/api/users', { 'username': username, 'password': password},
            function (result) {
                alert("Uspesno ste se prijavili");
                let userId = GetCookie("session-id");
                sessionStorage.setItem("userId", userId);
                ShowSessionContent();
                // uradi samo location.reload() ako se puno zakomplikuje ova funkcija
                //let user = JSON.parse(sessionStorage.getItem("user"));
                //alert(user.LastName);
            }
        ).fail(function (data) {
            alert(data.responseText);
        });
    });

    $("#logoutButton").click(function () {
        $.get('/api/users/', {'logout' : "yes"},
            function (result) {
                alert(result);
                let userId = GetCookie("session-id");
                sessionStorage.setItem("userId", userId);
                ShowGuestContent();
                // uradi samo location.reload() ako se puno zakomplikuje ova funkcija
                //let user = JSON.parse(sessionStorage.getItem("user"));
                //alert(user.LastName);
            }
        ).fail(function (data) {
            alert(data.responseText);
        });
    });

    function GetCookie(cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1);
            if (c.indexOf(name) != -1) return c.substring(name.length, c.length);
        }
        return "";
    } 

    function ShowSessionContent() {
        $("#logoutButton").show();
        $("#showEditTable").show();
        $("#editButton").show();
        $("#submitButton").hide();
        $("#showRegisterTableButton").hide();
        $("#showLoginTableButton").hide();
        $("#registerTable").hide();
        $("#loginTable").hide();
        $("#showRegisterTableButton").text("Registracija novog korisnika");
        $("#showLoginTableButton").text("Prijava korisnika");
        $("#showEditTable").text("Izmeni profil");
    }

    function ShowGuestContent() {
        $("#logoutButton").hide();
        $("#showEditTable").hide();
        $("#editButton").hide();
        $("#submitButton").show();
        $("#showRegisterTableButton").show();
        $("#showLoginTableButton").show();
        $("#registerTable").hide();
        $("#loginTable").hide();
        $("#showRegisterTableButton").text("Registracija novog korisnika");
        $("#showLoginTableButton").text("Prijava korisnika");
        $("#showEditTable").text("Izmeni profil");
    }
    
    function EmptyLoginFields() {
        $("#loginUsername").val("");
        $("#loginPassword").val("");
        $("#loginUsername").css("border", "1px solid black");
        $("#invalidLoginUsername").html("");
        $("#loginPassword").css("border", "1px solid black");
        $("#invalidLoginPassword").html("");
        isLoginUsernameValid = false;
        isLoginPasswordValid = false;
    }
})