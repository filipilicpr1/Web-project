$(document).ready(function () {
    let address = $(location).attr('href').split('?')[1];
    $.get("/api/fitnesscenters/", { 'address': address }, function (data, status) {
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