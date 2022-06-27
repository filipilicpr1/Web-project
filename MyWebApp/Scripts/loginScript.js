$(document).ready(function () {
    var isLoginUsernameValid = false;
    var isLoginPasswordValid = false;
    var userId = sessionStorage.getItem("userId");
    var userType = ["POSETILAC", "TRENER", "VLASNIK"];



    // event handler za kad se klikne na uloguj se
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

    // validacije za username i password
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

    // event handler za login
    $("#loginButton").click(function () {
        if (!isLoginPasswordValid || !isLoginUsernameValid) {
            $("#loginUsername").trigger('focusout');
            $("#loginPassword").trigger('focusout');
            return;
        }
        let username = $("#loginUsername").val();
        let password = $("#loginPassword").val();
        $.post('/api/users/login', { 'username': username, 'password': password},
            function (result) {
                alert("Uspesno ste se prijavili");
                userId = GetCookie("session-id");
                sessionStorage.setItem("userId", userId);
                ShowSessionContent(); // uradi samo location.reload() ako se puno zakomplikuje ova funkcija
            }
        ).fail(function (data) {
            alert(data.responseText);
        });
    });

    // even handler za logout
    $("#logoutButton").click(function () {
        $.get('/api/users/logout',
            function (result) {
                alert(result);
                let userId = GetCookie("session-id");   // na logout, cookie vrati "" za session-id
                sessionStorage.setItem("userId", userId);
                ShowGuestContent(); // uradi samo location.reload() ako se puno zakomplikuje ova funkcija
            }
        ).fail(function (data) {
            alert(data.responseText);
        });
    });

    // koristi se da izvuce session-id iz cookie, session id je id ulogovanog korisnika
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
        // korisnik je ulogovan
        // sakrij login, prikazi logout
        $("#logoutButton").show();
        $("#loginTable").hide();

        // sakrij registruj koriisnika, prikazi izmeni profil
        $("#showEditTable").show();
        $("#showRegisterTableButton").hide();

        // sakrij submit za registraciju, prikazi submit za izmenu
        $("#editButton").show();
        $("#submitButton").hide();

        // sakrij forme za registraciju i login
        $("#showLoginTableButton").hide();
        $("#registerTable").hide();

        // tekst dugmica koji se menja(x -> sakrij) postavi na inicijalnu vrednost
        $("#showRegisterTableButton").text("Registracija novog korisnika");
        $("#showLoginTableButton").text("Prijava korisnika");
        $("#showEditTable").text("Izmeni profil");
        $.get("/api/users", { 'id': userId }, function (data, status) {
            // dobavimo usera, pa odredimo koji je tip 
            let userIsVisitor = userType[data.UserType] == "POSETILAC"; // za sad se generise samo sadrzaj za posetioca
            let userIsTrainer = userType[data.UserType] == "TRENER";
            // u zavisnosti od tipa prikazemo odredjene linkove
            if (userIsVisitor) {
                $("#visitedGroupTrainingsLink").show();
            }

            if (userIsTrainer) {
                $("#visitedGroupTrainingsLink").show();
                $("#newGroupTrainingLink").show();
            }

        }).fail(function (data) {
            alert(data.responseJSON.Message);
        });
    }

    function ShowGuestContent() {
        // korisnik nije ulogovan
        // prikazi login, sakrij logout
        $("#logoutButton").hide();
        $("#showLoginTableButton").show();

        // prikazi registruj korisnika, sakrij izmeni profil
        $("#showEditTable").hide();
        $("#showRegisterTableButton").show();

        // prikazi submit za registraciju, sakrij submit za izmenu
        $("#editButton").hide();
        $("#submitButton").show();

        // sakrij forme za registraciju i login
        $("#registerTable").hide();
        $("#loginTable").hide();

        // tekst dugmica koji se menja(x -> sakrij) postavi na inicijalnu vrednost
        $("#showRegisterTableButton").text("Registracija novog korisnika");
        $("#showLoginTableButton").text("Prijava korisnika");
        $("#showEditTable").text("Izmeni profil");

        // sakrij sve linkove
        $("#visitedGroupTrainingsLink").hide();
        $("#newGroupTrainingLink").hide();
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