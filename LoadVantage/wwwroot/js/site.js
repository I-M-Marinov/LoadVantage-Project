// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



/*-------------------------------------------------------------------------------------
# HANDLE THE FADE OUT EFFECT OF SUCCESS AND ERROR MESSAGES IN ALL VIEWS 
--------------------------------------------------------------------------------------*/
document.addEventListener("DOMContentLoaded", function () {
    // Error message fade-out

    var errorMessage = document.getElementById('error-message');
    if (errorMessage) {
        setTimeout(function () {
            errorMessage.classList.add('fade-out');
        }, 1000); 
        setTimeout(function () {
            errorMessage.style.display = 'none'; 
        }, 3500); 
    }

    // Success message fade-out
    var successMessage = document.getElementById('success-message');
    if (successMessage) {
        setTimeout(function () {
            successMessage.classList.add('fade-out');
        }, 1000); 
        setTimeout(function () {
            successMessage.style.display = 'none'; 
        }, 3500); 
    }
});

/*-------------------------------------------------------------------------------------
# SAVE CHANGES BUTTON IN THE EDIT PROFILE SECTION IN THE USER'S PROFILE 
--------------------------------------------------------------------------------------*/

function showLoader() {

    document.getElementById("loader").style.display = "inline-block";

}

function showAndHideLoader() {

    document.getElementById("loader").style.display = "inline-block";
    setTimeout(() => {
        hideLoader();
    }, 350);
}

function hideLoader() {
    document.getElementById("loader").style.display = "none";
}



document.addEventListener("DOMContentLoaded", function () {
    var editLoadButton = document.getElementById('editLoadButton');

    if (editLoadButton) { // Only run if the button exists on the page
        editLoadButton.addEventListener('click', function (event) {
            enableFields();
        });
    }

    function enableFields() {
        setTimeout(() => {
            if (typeof showAndHideLoader === "function") {
                showAndHideLoader();
            }
            document.getElementById("originCity").disabled = false;
            document.getElementById("originState").disabled = false;
            document.getElementById("destinationCity").disabled = false;
            document.getElementById("destinationState").disabled = false;
            document.getElementById("pickupTime").disabled = false;
            document.getElementById("deliveryTime").disabled = false;
            document.getElementById("price").disabled = false;
            document.getElementById("weight").disabled = false;

            document.getElementById("editLoadButton").style.display = "none";

            const postLoadBtn = document.getElementById("postLoadBtn");

            if (postLoadBtn)
            { 
                postLoadBtn.style.display = "none";
            }



            if (document.getElementById("cancelLoadBtn")) {
                document.getElementById("cancelLoadBtn").style.display = "none";
            }
            if (document.getElementById("cancelCarrierBtn")) {
                document.getElementById("cancelCarrierBtn").style.display = "none";
            }

            document.getElementById("saveLoadButton").style.display = "inline-block";
            document.getElementById("cancelEditingButton").style.display = "inline-block";


            document.getElementById("isEditing").value = "true";
        }, 300);
    }
});



document.addEventListener("DOMContentLoaded", function () {
    var cancelEditingButton = document.getElementById('cancelEditingButton');


    if (cancelEditingButton) { // Only run if the button exists on the page
        cancelEditingButton.addEventListener('click', function (event) {
            dissableFieldsAndtoggleButtons();

        });
    }
   
    function dissableFieldsAndtoggleButtons() {
        const form = document.getElementById("loadDetailsForm");
        const isValid = $(form).valid(); 

        if (!isValid) {

            return;
        }

        setTimeout(() => {

            showAndHideLoader();

            document.getElementById("originCity").disabled = true;
            document.getElementById("originState").disabled = true;
            document.getElementById("destinationCity").disabled = true;
            document.getElementById("destinationState").disabled = true;
            document.getElementById("pickupTime").disabled = true;
            document.getElementById("deliveryTime").disabled = true;
            document.getElementById("price").disabled = true;
            document.getElementById("weight").disabled = true;

            document.getElementById("editLoadButton").style.display = "inline-block";

            const postLoadBtn = document.getElementById("postLoadBtn");

            if (postLoadBtn) {
                postLoadBtn.style.display = "inline-block";
            }
        
            if (document.getElementById("cancelLoadBtn")) {
                document.getElementById("cancelLoadBtn").style.display = "inline-block";
            }
          
            if (document.getElementById("cancelCarrierBtn")) {
                document.getElementById("cancelCarrierBtn").style.display = "inline-block";
            }


            document.getElementById("saveLoadButton").style.display = "none";
            document.getElementById("cancelEditingButton").style.display = "none";

        }, 300);
    }
});


document.addEventListener("DOMContentLoaded", function () {
    var cancelButton = document.getElementById('cancelLoadBtn');
    
    if (cancelButton) {  // Only run if the button exists on the page
        cancelButton.addEventListener('click', function (event) {
            event.preventDefault();

            showCancelConfirmationModal();
        });
    }
   
    function showCancelConfirmationModal() {
        var modal = document.getElementById('cancelConfirmationModal');
        var cancelBtn = document.getElementById('confirmCancelBtn');

        $(modal).modal('show');

        cancelBtn.addEventListener('click', function () {
            document.getElementById('cancelLoadForm').submit();
        });
    }
});

document.addEventListener("DOMContentLoaded", function () {
    const createLoadButton = document.getElementById("createLoadButton");

    if (createLoadButton) {
        createLoadButton.addEventListener("click", function () {

            const form = document.getElementById("creteLoadForm");
            const isValid = $(form).valid();

            if (!isValid) {

                showAndHideLoader();
                return;
            }

            showLoader();
            
        });
    }
});

document.addEventListener("DOMContentLoaded", function () {
    const editLoadButton = document.getElementById("editLoadButton");

    if (editLoadButton) {
        editLoadButton.addEventListener("click", function () {

            const form = document.getElementById("loadDetailsForm");
            const isValid = $(form).valid();

            if (!isValid) {

                showAndHideLoader();
                return;
            }

            showLoader();

        });
    }
});

document.addEventListener("DOMContentLoaded", function () {
    const saveLoadButton = document.getElementById("saveLoadButton");

    if (saveLoadButton) {
        saveLoadButton.addEventListener("click", function () {

            const form = document.getElementById("loadDetailsForm");
            const isValid = $(form).valid();

            if (!isValid) {

                showAndHideLoader();
                return;
            }

            showLoader();

        });
    }
});


/*-------------------------------------------------------------------------------------
# TOGGLE SIDEBAR ON AND OFF THE SCREEN 
--------------------------------------------------------------------------------------*/


if (document.querySelector('.toggle-sidebar-btn')) {

    document.querySelector('.toggle-sidebar-btn').addEventListener('click', function (e) {

        document.body.classList.toggle('toggle-sidebar');
    });
}

/*-------------------------------------------------------------------------------------
# JQUERY DEBUGGING AND ENSURE VALIDATION ON THE CHANGE PASSWORD FORM AND THE UPDATE PROFILE FORM
--------------------------------------------------------------------------------------*/

// Debugging purposes only
$(document).ready(function () {
    console.log("jQuery is loaded!");
});


$("#changePasswordForm").submit(function (event) {
    console.log("Submit event triggered");
    if (!$(this).valid()) {
        console.log("Form is invalid");
        event.preventDefault();
    }
});

$("#updateProfileForm").submit(function (event) {
    if (!$(this).valid()) {
        event.preventDefault();
    }
});

/*-------------------------------------------------------------------------------------
# SIGNALR CHAT HUB 
--------------------------------------------------------------------------------------*/


if (window.isUserAuthorized === "true") {

    const chatHubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    chatHubConnection.start().then(() => {
        console.log("SignalR ChatHub connection initialized.");

        // Check if the "chat-messages-container" exists
        const messageContainer = document.getElementById("chat-messages-container");
        if (!messageContainer) {
            console.warn("Chat messages container not found. Skipping message handling.");
            return;
        }

        // Handle incoming messages
        chatHubConnection.on("ReceiveMessage", function (chatMessage) {
            displayMessage(chatMessage);
        });

        // Attach submit event listener if the form exists
        const form = document.querySelector("form");
        if (form) {
            form.addEventListener("submit", function (event) {
                event.preventDefault();

                const messageContentInput = document.querySelector("input[name='messageContent']");
                const receiverIdInput = document.querySelector("input[name='receiverId']");
                const senderIdElement = document.getElementById("senderId");

                if (messageContentInput && receiverIdInput && senderIdElement) {
                    const messageContent = messageContentInput.value;
                    const receiverId = receiverIdInput.value;
                    const senderId = senderIdElement.value;

                    chatHubConnection.invoke("SendMessage", senderId, receiverId, messageContent)
                        .then(() => {
                            console.log("Message sent successfully.");
                            messageContentInput.value = "";
                        })
                        .catch(err => {
                            console.error("Error sending message: " + err);
                        });
                } else {
                    console.error("Form inputs are missing.");
                }
            });
        } else {
            console.warn("Form element not found. Skipping form submission handling.");
        }

        // Mark messages as read when clicked
        messageContainer.addEventListener('click', function (e) {
            const messageElement = e.target.closest('.chat-message');
            if (messageElement && messageElement.classList.contains('received')) {
                const senderId = messageElement.getAttribute('data-sender-id');
                const receiverId = messageElement.getAttribute('data-receiver-id');

                markMessagesAsRead(senderId, receiverId);

                messageElement.classList.add('read');
            }
        });
    }).catch(err => {
        console.error("Error initializing SignalR connection: " + err);
    });

    // Sanitize the message
    function sanitizeMessage(message) {
        const element = document.createElement('div');
        element.innerText = message;
        return element.innerHTML;
    }

    // Display the message
    function displayMessage(chatMessage) {
        const messageContainer = document.getElementById("chat-messages-container");
        if (!messageContainer) {
            console.warn("Message container not found. Cannot display message.");
            return;
        }

        const messageElement = document.createElement("div");
        messageElement.classList.add("chat-message");

        const senderId = document.getElementById("senderId")?.value;
        if (chatMessage.senderId === senderId) {
            messageElement.classList.add("sent");
            messageElement.setAttribute("data-is-received", "false");
        } else {
            messageElement.classList.add("received");
            messageElement.setAttribute("data-is-received", "true");
        }

        messageElement.setAttribute("data-message-id", chatMessage.id);
        messageElement.setAttribute("data-sender-id", chatMessage.senderId);
        messageElement.setAttribute("data-receiver-id", chatMessage.receiverId);

        const messageContent = document.createElement("div");
        messageContent.classList.add("message-content");
        messageContent.innerHTML = sanitizeMessage(chatMessage.content); // Sanitization

        const messageTimestamp = document.createElement("div");
        messageTimestamp.classList.add("message-timestamp");
        messageTimestamp.innerText = new Date(chatMessage.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

        messageElement.appendChild(messageContent);
        messageElement.appendChild(messageTimestamp);

        if (chatMessage.isRead) {
            messageElement.classList.add("read");
        }

        messageContainer.appendChild(messageElement);

        // Scroll to the bottom of the message container
        messageContainer.scrollTop = messageContainer.scrollHeight;
    }
};



