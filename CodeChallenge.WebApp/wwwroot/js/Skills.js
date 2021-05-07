var urlStandard = window.location.origin;
var allSkills = [];
class Skill {
    constructor(id, name, description, isRequired, dateCreated, employees) {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.IsRequired = isRequired;
        this.DateCreated = dateCreated;
        this.Employees = employees;
    }
}
$(document).ready(function () {
    GetAllSkills();
});
function GetAllSkills() {
    allSkills = [];
    $.ajax({
        url: urlStandard + '/skills/GetAllSkills',
        type: 'GET',
        success: function (res) {
            res.forEach(i => {
                allSkills.push(i);
            });
        }
    });
}
function RecalculateAllSkills() {
    var recalculateSkills = false;
    allSkills.forEach(a => {
        if (a.Id == 0) {
            recalculateSkills = true;
        }
    })
    if (recalculateSkills)
        GetAllSkills();
}
function EditingStart(data) {
    RecalculateAllSkills();
    $(document).ready(function () {
        $('#id input').val(data.key);
        $("#name").dxTextBox({
            value: data.data.Name
        }).dxValidator({
            validationGroup: "EditPopUp"
        });
        $("#description").dxTextArea({
            value: data.data.Description
        }).dxValidator({
            validationGroup: "EditPopUp"
        });
        $("#required").dxCheckBox({
            value: data.data.IsRequired
        });
        $("#dateCreated").dxDateBox({
            value: Date.parse(data.data.DateCreated),
            min: data.data.DateCreated
        });      
        $("#SavePopUp").dxButton({
            validationGroup: "EditPopUp"
        });
    });
}
function Edit() {
    $("#name").dxTextBox({
        readOnly: false
    });
    $("#description").dxTextArea({
        readOnly: false
    });
    $("#dateCreated").dxDateBox({
        readOnly: false
    });
    $("#required").dxCheckBox({
        readOnly: false
    });
}
function Hiding() {
    $("#name").dxTextBox({
        readOnly: true
    });
    $("#description").dxTextArea({
        readOnly: true
    });
    $("#dateCreated").dxDateBox({
        readOnly: true
    });
    $("#required").dxCheckBox({
        readOnly: true
    });
    $('#name').dxValidator('instance').reset();
    $('#description').dxValidator('instance').reset();
}
function Cancel() {
    Hiding();
    $("#EditSkillPopUp").dxPopup("hide");
}
function Save(e) {
    ValidateForm();
    var result = e.validationGroup.validate();
    if (result.isValid) {
        var skill = new Skill($('#id input').val(), $('#name input').val(), $('#description').dxTextArea('instance').option('value'), $("#required").dxCheckBox('instance').option('value'), $("#dateCreated input").val(), null)
        for (var i = 0; i < allSkills.length; i++) {
            if (allSkills[i].Id == $('#id input').val()) {
                allSkills[i].Name = $('#name input').val();
                allSkills[i].Description = $('#description').dxTextArea('instance').option('value');
            }
        }
        $.ajax({
            url: urlStandard + "/skills/EditSkill",
            type: 'PUT',
            data: JSON.stringify(JSON.stringify(skill)),
            contentType: 'application/json',
            success: function (res) {
                Cancel();
                $("#EditSkillPopUp").dxPopup("hide");
                $("#SkillGridContainer").dxDataGrid("getDataSource").reload();
            }
        });
    }
}
function Delete() {
    $("#deleteConfirm").dxPopup("instance").show();
}
function ToolbarPreparing(e) {
    e.toolbarOptions.items.unshift(
        {
            location: "before",
            widget: "dxTextBox",
            options: {
                value: "Skills",
                readOnly: true,
                stylingMode: "filled",
                width: 50
            }
        },
        {
        location: "after",
        widget: "dxButton",
        options: {
            icon: "refresh",
            onClick: function () {
                $("#SkillGridContainer").dxDataGrid("getDataSource").reload();
            }
        }
        },
        {
            location: "after",
            widget: "dxButton",
            options: {
                icon: "plus",
                onClick: function () {
                    RecalculateAllSkills();
                    $("#NewSkillPopUp").dxPopup("show");
                    $("#nameNew").dxTextBox().dxValidator({
                        validationGroup: "NewPopUp"
                    });
                    $("#descriptionNew").dxTextArea().dxValidator({
                        validationGroup: "NewPopUp"
                    });
                    $("#SavePopUpNew").dxButton({
                        validationGroup: "NewPopUp"
                    });
                }
            }
        });
}
function SaveNew(e) {
    ValidateFormNew();
    var result = e.validationGroup.validate();
    if (result.isValid) {
        var skill = new Skill(0, $('#nameNew input').val(), $('#descriptionNew').dxTextArea('instance').option('value'), $("#requiredNew").dxCheckBox('instance').option('value'), $("#dateCreatedNew input").val(), null)
        allSkills.push(skill);
        $.ajax({
            url: urlStandard + "/skills/CreateSkill",
            type: 'POST',
            data: JSON.stringify(JSON.stringify(skill)),
            contentType: 'application/json',
            success: function (res) {
                CancelNew();
                $("#NewSkillPopUp").dxPopup("hide");
                $("#SkillGridContainer").dxDataGrid("getDataSource").reload();
            }
        });
    }
}
function HidingNew() {
    $("#NewSkillPopUp").dxPopup("hide");
    $('#nameNew input').val('');
    $("#requiredNew").dxCheckBox({
        value: true
    });
    $('#descriptionNew').dxTextArea({
        value: ''
    });
    $("#dateCreatedNew").dxDateBox({
        value: new Date()
    });
    $('#nameNew').dxValidator('instance').reset();
    $('#descriptionNew').dxValidator('instance').reset();
}
function CancelNew() {
    HidingNew();
    $("#NewSkillPopUp").dxPopup("hide");
}
function ConfirmDelete() {
    var indexDelete = 0;
    allSkills.forEach((a, i) => {
        if (a.Name == $('#name input').val()) {
            indexDelete = i;
        }
    });
    allSkills.splice(indexDelete, 1);
    $.ajax({
        url: urlStandard + "/skills/DeleteSkill?id=" + $('#id input').val(),
        type: 'DELETE',
        contentType: 'application/json',
        success: function (res) {
            $("#EditSkillPopUp").dxPopup("hide");
            $("#SkillGridContainer").dxDataGrid("getDataSource").reload();
            $("#deleteConfirm").dxPopup("instance").hide();
        }
    });
}
function ClosePopUpDelete() {
   $("#deleteConfirm").dxPopup("instance").hide();
}
function PageSize() {
    var mainHeight = $('main')[0].offsetHeight;
    var toolbarAndPagingHeight = 101;
    var headerRowHeight = 33;
    var gridHeight = mainHeight * 0.9 - toolbarAndPagingHeight - headerRowHeight;
    var pages = gridHeight / 33;
    return pages|0;
}
function ValidateForm() {
    $("#name").dxTextBox({ value: $('#name input').val() }).dxValidator({
        validationGroup: "EditPopUp",
        validationRules: [{
            type: "required",
            message: "Field is required"
        },
        {
            type: "stringLength",
            min: 1,
            max: 64,
            message: "Maximum length 64 characters"
        },
        {
            type: "custom",
            validationCallback: function (e) {
                var newName = e.value;
                var id = $('#id input').val();
                var itemsInGrid = allSkills;
                var count = 0;
                itemsInGrid.forEach(i => {
                    if (i.Name == newName && i.Id!=id)
                        count++;
                })
                return count==0;
            },
            message: "Name already exists"
        }]
    });
       
    $("#description").dxTextArea({ value: $('#description').dxTextArea('instance').option('value') })
        .dxValidator({
            validationGroup: "EditPopUp",
            validationRules: [{
                type: "required",
                message: "Field is required"
            },
            {
                type: "stringLength",
                min: 1,
                max: 256,
                message: "Maximum length 256 characters"
            },
            {
                type: "custom",
                validationCallback: function (e) {
                    var newDescription = e.value;
                    var id = $('#id input').val();
                    var itemsInGrid = allSkills;
                    var count = 0;
                    itemsInGrid.forEach(i => {
                        if (i.Description == newDescription && i.Id!=id)
                            count++;
                    })
                    return count==0;
                },
                message: "Description already exists"
            }]
        });

    $("#SavePopUp").dxButton({
        validationGroup: "EditPopUp"
    });
}
function ValidateFormNew() {
    $("#nameNew").dxTextBox({ value: $('#nameNew input').val() }).dxValidator({
        validationGroup: "NewPopUp",
        validationRules: [{
            type: "required",
            message: "Field is required"
        },
        {
            type: "stringLength",
            min: 1,
            max: 64,
            message: "Maximum length 64 characters"
        },
        {
            type: "custom",
            validationCallback: function (e) {
                var newName = e.value;
                var itemsInGrid = allSkills;
                var count = 0;
                itemsInGrid.forEach(i => {
                    if (i.Name == newName)
                        count++;
                })
                return count == 0;
            },
            message: "Name already exists"
        }]
    });
    $("#descriptionNew").dxTextArea({ value: $('#descriptionNew').dxTextArea('instance').option('value') }).dxValidator({
            validationGroup: "NewPopUp",
            validationRules: [{
                type: "required",
                message: "Field is required"
            },
            {
                type: "stringLength",
                min: 1,
                max: 256,
                message: "Maximum length 256 characters"
            },
            {
                type: "custom",
                validationCallback: function (e) {
                    var newDescription = e.value;
                    var itemsInGrid = allSkills;
                    var count = 0;
                    itemsInGrid.forEach(i => {
                        if (i.Description == newDescription)
                            count++;
                    })
                    return count == 0;
                },
                message: "Description already exists"
            }]
    });
    $("#SavePopUpNew").dxButton({
        validationGroup: "NewPopUp"
    });
}


