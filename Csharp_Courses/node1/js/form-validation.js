function validateForm(func) {
    if (func == "view" || func == "regReq" || func == "remove" || func == "offer"){
        var user = document.forms["myform"]["ID"].value;
        if (user == null || user.length != 3) {
            alert("ID error");
            return false;
        }
    }
    if (func == "view1" || func == "regReq" || func == "remove" || func == "offer" || func == "CheckAvailable") {
        var want = document.forms["myform"]["want"].value;
        if (want == null || want.valueOf() > 10 || want.valueOf() < 1) {
            alert("Course Num error");
            return false;
        }
    }
    if (func == "offer") {
        var offer = document.forms["myform"]["offer"].value;
        if (offer.valueOf() > 10 || offer.valueOf() < 1) {
            alert("Course Num error");
            return false;
        }
    }

    var pass = document.forms["myform"]["password"].value;
    if ((func == "view" || func == "regReq" || func == "remove" || func == "offer" || func == view1) && pass.length != 2) {
        alert("Password must be 2 digits");
        return false;
    }
    return true;
}
