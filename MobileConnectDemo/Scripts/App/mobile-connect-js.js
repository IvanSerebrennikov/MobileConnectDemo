$(function() {
    $("#mobileConnectAuthSetup").on("click", "#mobileConnectAuthSubmit", function () {
        var phoneNumber = $("#mobileConnectAuthSetup #phoneNumber").val();

        if (!phoneNumber)
            return;

        var redirectUrl = $("#mobileConnectAuthSetup #redirectUrl").val();

        if (!redirectUrl)
            return;

        var notificationUri = $("#mobileConnectAuthSetup #notificationUri").val();

        if (!notificationUri)
            return;

        var discoveryUrl = $("#mobileConnectAuthSetup #discoveryUrl").val();

        if (!discoveryUrl)
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
            notificationUri: notificationUri,
            discoveryUrl: discoveryUrl,
            discoveryClientId: discoveryClientId,
            discoveryPassword: discoveryPassword
        };

        $.post("/MobileConnect/Authorize", requestData)
            .done(function(responseConetnt) {
                if (responseConetnt) {
                    $("#mobileConnectAuthResult").html(responseConetnt);
                }
            });
    });
});