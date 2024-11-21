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
# GET UNREAD MESSAGES AND THEIR COUNT
--------------------------------------------------------------------------------------*/


//function updateUnreadCount() {
//    $.get('/Chat/GetUnreadMessages', function (data) {

//        let unreadCount = 0;

//        if (data.unreadCount != null) {
//            unreadCount = data.unreadCount;

//        }
//        const unreadMessages = data.messages;

//        // Update the notification counts
//        document.getElementById('notificationCount').textContent = unreadCount;
//        document.getElementById('loadNotificationCount').textContent = unreadCount;

//        // Clear the current notification list
//        const notificationList = document.getElementById('loadNotificationList');
//        notificationList.innerHTML = ''; // Clear existing items

//        // Add new notifications
//        unreadMessages.forEach((message) => {
//            const notificationItem = createNotificationItem(message);
//            notificationList.appendChild(notificationItem);
//        });
//    }).fail(function (error) {
//        console.error("Error fetching unread messages:", error);
//    });
//}

//function createNotificationItem(chatMessage) {
//    // Validate message data
//    if (!chatMessage.senderId || !chatMessage.receiverId) {
//        console.error("Invalid chat message data:", chatMessage);
//        return null;
//    }

//    // Create list item
//    const notificationItem = document.createElement('li');
//    notificationItem.classList.add('message-item');

//    // Create link
//    const link = document.createElement('a');
//    link.href = '#';
//    link.classList.add('notification-link'); // Optional class for styling
//    link.dataset.senderId = chatMessage.senderId; // Store sender ID
//    link.dataset.receiverId = chatMessage.receiverId; // Store receiver ID

//    // Attach click event to the link
//    link.addEventListener('click', function (event) {
//        event.preventDefault(); // Prevent default link behavior
//        openChat(chatMessage.senderId, chatMessage.receiverId);
//    });

//    // Create message body
//    const messageBody = document.createElement('div');
//    messageBody.classList.add('message-body');

//    // Create title
//    const messageTitle = document.createElement('h6');
//    messageTitle.classList.add('message-title');
//    messageTitle.textContent = chatMessage.content;

//    // Create timestamp
//    const messageTime = document.createElement('p');
//    messageTime.classList.add('message-time');
//    messageTime.textContent = new Date(chatMessage.timestamp).toLocaleTimeString();

//    // Append title and timestamp to the body
//    messageBody.appendChild(messageTitle);
//    messageBody.appendChild(messageTime);

//    // Append body to the link
//    link.appendChild(messageBody);

//    // Append the link to the notification item
//    notificationItem.appendChild(link);

//    return notificationItem;
//}

//function openChat(senderId, receiverId) {
//    $.post('/Chat/ChatWithBrokerWindow',
//        { senderId: senderId, receiverId: receiverId },
//        function () {
//            console.log("Messages marked as read.");
//            updateUnreadCount(); // Refresh unread count after marking messages
//        }
//    ).fail(function (error) {
//        console.error("Error marking messages as read:", error);
//    });
//}





