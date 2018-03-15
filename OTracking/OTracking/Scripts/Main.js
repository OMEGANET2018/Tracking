function alerta(Mensaje, color) {
    if ($('.alert').css("display") == "none") {
        $('.alert').show();
        if (Mensaje != undefined)
            $('.alert span').html(Mensaje);

        var stilo = "";
        if (color != undefined) {
            switch (color) {
                case "negro": {
                    stilo = "alert-dark";
                    break;
                }
                case "blanco": {
                    stilo = "alert-light";
                    break;
                }
                case "rojo": {
                    stilo = "alert-danger";
                    break;
                }
                case "amarillo": {
                    stilo = "alert-warning";
                    break;
                }
                case "azul": {
                    stilo = "alert-info";
                    break;
                }
                case "verde": {
                    stilo = "alert-success";
                    break;
                }
            }
            $('.alert').addClass(stilo);
        }

        window.setTimeout(function () {
            $(".alert").fadeTo(500).slideUp(500, function () {
                if (stilo != "")
                    $('.alert').removeClass(stilo);
                $(this).hide();
            });
        }, 1000);
    }
    else {
        $('.alert').hide();
    }
}

function validateNumber(evt) {
    var theEvent = evt || window.event;
    var key = theEvent.keyCode || theEvent.which;
    key = String.fromCharCode(key);
    var regex = /[0-9]|\./;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}