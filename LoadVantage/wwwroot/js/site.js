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

            document.getElementById("cancelLoadBtn").style.display = "none";

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
        setTimeout(() => {

            const form = document.getElementById("loadDetailsForm"); 
            const isValid = $(form).valid(); 

            if (!isValid) {
         
                return; 
            }

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

            document.getElementById("cancelLoadBtn").style.display = "inline-block";

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
# SIGNALR WEB SOCKET CONNECTION TO RETRIEVE ANY NEW MESSAGES LOADS 
--------------------------------------------------------------------------------------*/

// Define a global connection variable
let messageConnection = null;

// Initialize SignalR and connect
function initializeMessageNotifications() {
    const messageNotificationCount = document.getElementById("messageNotificationCount");
    const messageNotificationList = document.getElementById("messageNotificationList");
    const messageCountDisplay = document.getElementById("messageCount");

    // Check if connection already exists
    if (!messageConnection) {
        messageConnection = new signalR.HubConnectionBuilder()
            .withUrl("/chatHub") // Ensure this matches your SignalR hub URL
            .build();

                .start()
            .then(() => {
                console.log("Connected to ChatHub.");
            })
            .catch(err => {
                console.error("Error connecting to ChatHub: " + err);
            });

        // Listen for new message notifications
        messageConnection.on("ReceiveMessageNotification", function (message) {
            console.log("New message received: ", message);
            addMessageNotification(message);
        });

        // Listen for receiving messages in real-time
        messageConnection.on("ReceiveMessage", function (message) {
            console.log("Received message: ", message);
            appendMessageToChat(message);
        });
    }
}

// Send message via SignalR
function sendMessage(receiverId, message) {
    if (!message || !receiverId) return;

    messageConnection.invoke("SendMessage", receiverId, message)
        .then(() => {
            console.log("Message sent successfully!");
            appendMessageToChat(message);
        })
        .catch(err => {
            console.error("Error sending message:", err);
        });
}

// Append the message to the chat window
function appendMessageToChat(message) {
    const chatMessages = document.getElementById("chat-messages");
    const messageElement = document.createElement("div");
    messageElement.classList.add("message-item");
    messageElement.innerHTML = `<strong>You:</strong> ${message}`;
    chatMessages.appendChild(messageElement);

    // Scroll to the latest message
    chatMessages.scrollTop = chatMessages.scrollHeight;
}

// Handle the form submission for sending messages
function handleSendMessage(event) {
    event.preventDefault();

    const messageInput = document.getElementById("message-input");
    const receiverId = document.getElementById("receiverId").value;  // Make sure this input exists and has the receiver's ID

    const message = messageInput.value;

    if (message) {
        sendMessage(receiverId, message);
        messageInput.value = "";  // Clear input after sending
    }
}

// Event listener for sending a message
document.getElementById("chat-form").addEventListener("submit", handleSendMessage);



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


