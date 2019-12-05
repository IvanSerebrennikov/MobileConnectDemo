$(function() {
    $("#mobileConnectAuthSetup").on("click", "#mobileConnectAuthSubmit", function () {
        var phoneNumber = $("#mobileConnectAuthSetup #phoneNumber").val();

        if (!phoneNumber)
            return;

        var redirectUrl = $("#mobileConnectAuthSetup #redirectUrl").val();

        if (!redirectUrl)
            return;

        var discoveryClientId = $("#mobileConnectAuthSetup #discoveryClientId").val();

        if (!discoveryClientId)
            return;

        var discoveryPassword = $("#mobileConnectAuthSetup #discoveryPassword").val();

        if (!discoveryPassword)
            return;

        var requestData = {
            phoneNumber: phoneNumber,
            redirectUrl: redirectUrl,
            discoveryClientId: discoveryClientId,
            discoveryPassword: discoveryPassword
        };

        $.post("/Home/MobileConnectAuthorize", requestData)
            .done(function(responseConetnt) {
                if (responseConetnt) {
                    $("#mobileConnectAuthResult").html(responseConetnt);
                }
            });
    });
});