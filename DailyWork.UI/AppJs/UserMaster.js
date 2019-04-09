$(document).ready(function () {
    $("#btnNew").on("click", function () {
        debugger
        alert(1);
        toastr["success"]("Hello", "Welcome to my site");
    });
    $("#btnAddUser").on("click", function () {
        toastr["success"]("Welcome", "Welcome to my site");
        AddUser();
    });
});




function AddUser() {
    var flag = true;
    var firstName = $("#txtFirstName").val();
    var lastName = $("#txtLastName").val();
    var gender = $("#ddlGender option:selected").val();
    var dateofBirth = $("#dtDateofbirth").val();
    var email = $("#txtEmail").val();
    var mobile = $("#txtMobile").val();
    var password = $("#txtPassword").val();

    if (firstName === null) {
        toastr.error["error"]("First name is required");
        flag = false;
    }
    if (lastName === null) {
        toastr.error["error"]("Last name is required");
        flag = false;
    }
    if (gender === null) {
        toastr.error["error"]("Gender is required");
        flag = false;
    }
    if (dateofBirth === null) {
        toastr.error["error"]("Date of birth is required");
        flag = false;
    }
    if (email === null) {
        toastr.error["error"]("Email is required");
        flag = false;
    }
    if (mobile === null) {
        toastr.error["error"]("Mobile is required");
        flag = false;
    }
    if (password === null) {
        toastr.error["error"]("Password is required");
        flag = false;
    }

    var formData = new FormData();
    formData.append("FirstName", firstName);
    formData.append("LastName", lastName);
    formData.append("Gender", gender);
    formData.append("DateOfBirth", dateofBirth);
    formData.append("Email", email);
    formData.append("Mobile", mobile);
    formData.append("Password", password);

    if (flag) {
        $.ajax({
            url: "/User/Save",
            type: "POST",
            dataType: "json",
            contentType: false,
            processData: false,
            data: formData,
            success: function (response) {
                toastr.success("User is added");
                window.location.href = "/User/Index";
            }
        });
    }
}