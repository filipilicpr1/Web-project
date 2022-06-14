$(document).ready(function () {
    let id = $(location).attr('href').split('?')[1];
    $.get("/api/fitnesscenters/", { 'id': id }, function (data, status) {
        $("#naziv").html(data.Name);
        $("#adresa").html(data.Address);
        $("#godina").html(data.YearCreated);
        $("#vlasnik").html(data.Owner.Name + " " + data.Owner.LastName);
        $("#mesecna").html(data.MonthlySubscription);
        $("#godisnja").html(data.YearlySubscription);
        $("#jedanTrening").html(data.TrainingCost);
        $("#grupniTrening").html(data.GroupTrainingCost);
        $("#personalniTrening").html(data.PersonalTrainingCost);
    });

    $.get("/api/grouptrainings", { 'fitnessId': id }, function (data, status) {
        let tableContent = "<table border='1'><caption align='center'>Predstojeci grupni treninzi</caption><tr><th>Naziv</th><th>Vrsta treninga</th><th>Trajanje</th><th>Datum</th><th>Vreme</th><th>Kapacitet</th><th>Broj prijavljenih</th></tr>";
        for (groupTraining in data) {
            tableContent += `<tr><td>${data[groupTraining].Name}</td><td>${data[groupTraining].TrainingType}</td><td>${data[groupTraining].Duration}</td><td>${data[groupTraining].DateOfTraining.slice(0, 10)}</td><td>${data[groupTraining].DateOfTraining.slice(11, 16)}</td><td>${data[groupTraining].VisitorCapacity}</td><td>${data[groupTraining].VisitorCount}</td></tr>`;
        }
        tableContent += "</table>";
        $("#fitnessCenterGroupTrainingsTable").html(tableContent);
    });

    $.get("/api/comments", { 'fitnessId': id }, function (data, status) {
        let tableContent = "<table border='1'><caption align='center'>Komentari</caption><tr><th>Komentar</th><th>Ocena</th></tr>";
        for (comment in data) {
            tableContent += `<tr><td>${data[comment].Text}</td><td>${data[comment].Rating}</td></tr>`;
        }
        tableContent += "</table>";
        $("#fitnessCenterCommentsTable").html(tableContent);
    });
})