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
})