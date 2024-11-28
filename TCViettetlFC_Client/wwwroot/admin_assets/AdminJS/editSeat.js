





function updateSeat(AreaId, MatchId) {

    var quantittySeat = $("#seat_" + AreaId).val();

    var data = {
        AreaId: AreaId,
        MatchId: MatchId,
        SeatQuantity: quantittySeat
    }
    $.ajax({
        url: 'https://localhost:5000/api/MatchAreas/UpdateSeat' ,
        type: 'Put',
        contentType: 'application/json', 
        data: JSON.stringify(data),
        success: function (response) {
          /*  loadData();*/
            showAlert("Cập nhật thành công");
            //$("#modalEditorCreate").modal("hide");
        },
        error: function (error) {
            debugger
        }
    });

}




function showAlert(mess) {

    $("#alertMessageSuccess").text(mess);
    $("#alertSuccess").show();
    setTimeout(function () {
        $("#alertSuccess").hide(); 
    }, 3500);
}


