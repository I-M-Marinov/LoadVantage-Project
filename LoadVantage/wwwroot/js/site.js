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

function handleSaveButtonClick(event) {
    event.preventDefault();

    const button = document.getElementById('saveProfileButton');
    const spinner = document.getElementById('buttonSpinner');
    const buttonText = document.getElementById('buttonText');

    spinner.style.display = 'inline-block';
    buttonText.textContent = 'Saving...';
    button.disabled = true;

    setTimeout(() => {
        buttonText.textContent = 'Saved';

        setTimeout(() => {
            buttonText.textContent = 'Save Changes';
            spinner.style.display = 'none';
            button.disabled = false;
        }, 1500); 
    }, 1500); 

    document.getElementById('updateProfileForm').submit();
}
    