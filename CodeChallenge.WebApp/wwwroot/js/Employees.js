var urlStandard = window.location.origin;
var allSkills = [];
var allSkillsOfEmployees = [];
var skillsInEditpopUp = [];
var allEmployees = [];
var importedEmployees = [];
var allowSave = true;
var allowSaveNew = true;
var startingFieldForEmployeeValidation = '';

class Employee {
    constructor(id, name, surname, hiringDate, skillset, dateSkillChanged, typeOfSkillChange) {
        this.Id = id;
        this.Name = name;
        this.Surname = surname;
        this.HiringDate = hiringDate;
        this.Skillset = skillset;
        this.DateSkillChanged = dateSkillChanged;
        this.TypeOfSkillChange = typeOfSkillChange;
    }
}
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
    GetAllEmployees();
});
function GetAllSkills() {
    allSkills = [];
    $.ajax({
        url: urlStandard + '/skills/GetAllSkills',
        type: 'GET',
        success: function (res) {
            allSkills = [];
            res.forEach(i => {
                allSkills.push(i);
            });
        }
    });
}
function GetAllEmployees() {
    allEmployees = [];
    $.ajax({
        url: urlStandard + '/employees/Get',
        type: 'GET',
        success: function (res) {
            allEmployees = [];
            res.data.forEach(i => {
                allEmployees.push(i);
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
function RecalculateAllEmployees() {
    var recalculateEmployees = false;
    allEmployees.forEach(a => {
        if (a.Id == 0) {
            recalculateEmployees = true;
        }
    })
    if (recalculateEmployees)
        GetAllEmployees();
}
function EditingStart(data) {
    RecalculateAllSkills();
    RecalculateAllEmployees();
    $(document).ready(function () {
        DataInSkillsGrid(data.key); 
        $('#id input').val(data.key);
        $("#name").dxTextBox({
            value: data.data.Name
        }).dxValidator({
            validationGroup: "EditPopUp"
        });
        $("#surname").dxTextBox({
            value: data.data.Surname
        }).dxValidator({
            validationGroup: "EditPopUp"
        });
        $("#hiringDate").dxDateBox({
            value: Date.parse(data.data.HiringDate)
        }).dxValidator({
            validationGroup: "EditPopUp"
        });
        $("#dateSkillChanged").dxDateBox({
            value: Date.parse(data.data.DateSkillChanged)
        });
        $('#typeOfSkillChange input').val(data.data.TypeOfSkillChange);
        $("#SavePopUp").dxButton({
            validationGroup: "EditPopUp"
        });
    });
}
function Edit() {
    $("#name").dxTextBox({
        readOnly: false
    });
    $("#surname").dxTextBox({
        readOnly: false
    });
    $("#hiringDate").dxDateBox({
        readOnly: false
    });
    $('#SkillsGridContainerEdit').dxDataGrid('instance').option('editing.allowDeleting', true);
    $('#SkillsGridContainerEdit').dxDataGrid('instance').option('editing.allowAdding', true)
}
function Save(e) {
    ValidateForm();
    var result = e.validationGroup.validate();
    $("#SkillsGridContainerEdit").dxDataGrid("instance").getController("validating").validate(true);
    if (result.isValid && allowSave) {
        var newSkills = [];
        var removedSkills = [];
        var inserted = false;
        var removed = false;
        ($("#SkillsGridContainerEdit").dxDataGrid("instance").option("editing.changes")).forEach(x => {
            var skill;
            if (x.type == "insert") {
                inserted = true;
                skill = new Skill(x.data.Id, x.data.Name, x.data.Description, false, new Date(), null);
                newSkills.push(skill);
                if (x.data.Id == 0)
                    allSkills.push(x.data);

            }
            else if (x.type == "remove") {
                removed = true;
                var skillRemove;
                allSkills.forEach(a => {
                    if (a.Id == x.key) {
                        skillRemove = a;
                    }
                })
                var duplicateExists = 0;
                allEmployees.forEach(ii => {
                    ii.Skillset.forEach(i => {
                        if (i.Name == skillRemove.Name)
                            duplicateExists++;
                    })
                })
                if (duplicateExists==1)
                   allSkills.pop(skillRemove);
                newSkills.push(new Skill(skillRemove.Id, '', '', false, new Date(), null));
                removedSkills.push(skillRemove.Name);
            }
        });
        var typeSkillChange = "";
        if (inserted && removed)
            typeSkillChange = "Skill Removed and Added"
        else if (inserted)
            typeSkillChange = "Skill Added"
        else if (removed)
            typeSkillChange = "Skill Removed"
        var employee;
        if (typeSkillChange == "")
            employee = new Employee($('#id input').val(), $('#name input').val(), $('#surname input').val(), $("#hiringDate input").val(), newSkills, $("#dateSkillChanged input").val(), $('#typeOfSkillChange input').val());
        else
            employee = new Employee($('#id input').val(), $('#name input').val(), $('#surname input').val(), $("#hiringDate input").val(), newSkills, new Date(), typeSkillChange);
        for (var i = 0; i < allEmployees.length; i++) {
            if (allEmployees[i].Id == $('#id input').val()) {
                allEmployees[i].Name = $('#name input').val();
                allEmployees[i].Surname = $('#surname input').val();
                newSkills.forEach(s => {
                    if (s.Name != '' && s.Description!='')
                        allEmployees[i].Skillset.push(s);
                });
                var removed = true;
                allEmployees[i].Skillset.forEach((a,index) => {
                    var skillRemove;
                    removedSkills.forEach(r => {
                        if (r == a.Name) {
                            skillRemove = index;
                        }
                    });
                    if (skillRemove != undefined)
                        allEmployees[i].Skillset.splice(skillRemove, 1);
                })
            }
        }
        $.ajax({
            url: urlStandard + "/employees/EditEmployee?ids=",
            type: 'PUT',
            data: JSON.stringify(JSON.stringify(employee)),
            contentType: 'application/json',
            success: function (res) {
                Cancel();
                $("#EmployeesGridContainer").dxDataGrid("getDataSource").reload();
            }
        })
    }
}
function Delete() {
    $("#DeleteConfirm").dxPopup("instance").show();
}
function Cancel() {
    Hiding();
    $("#EditEmployeePopUp").dxPopup("hide");
}
function Hiding() {
    $("#name").dxTextBox({
        readOnly: true
    });
    $("#surname").dxTextBox({
        readOnly: true
    });
    $("#hiringDate").dxDateBox({
        readOnly: true
    });
    $("#dateSkillChanged").dxDateBox({
        readOnly: true
    });
    $("#typeOfSkillChange").dxTextBox({
        readOnly: true
    });
    $('#name').dxValidator('instance').reset();
    $('#surname').dxValidator('instance').reset();
    var grid = $("#SkillsGridContainerEdit").dxDataGrid("instance");
    if (grid != undefined) {
        grid.option('paging.pageIndex', 0);
        grid.option('editing.allowDeleting', false);
        grid.option('editing.allowAdding', false);
        grid.option("focusedRowIndex", -1); 
    }
}
function ConfirmDelete() {
    var removeEmployee;
    allEmployees.forEach((x,index) => {
        if (x.Name == $('#name input').val())
            removeEmployee = index;
    });
    if (removeEmployee != undefined)
        allEmployees.splice(removeEmployee, 1);
    $.ajax({
        url: urlStandard+"/employees/DeleteEmployee?id=" + $('#id input').val(),
        type: 'DELETE',
        contentType: 'application/json',
        success: function (res) {
            $("#EditEmployeePopUp").dxPopup("hide");
            $("#DeleteConfirm").dxPopup("instance").hide();
            $("#EmployeesGridContainer").dxDataGrid("getDataSource").reload();
        }
    })
}
function ConfirmDeleteMany() {
    var ids = $('#EmployeesGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    var data = $('#EmployeesGridContainer').dxDataGrid('instance').getSelectedRowsData();
    var removeEmployees=[];
    allEmployees.forEach((x, index) => {
        data.forEach(i => {
            if (i.Name == x.Name)
                removeEmployees.push(x);
        })
    });
    removeEmployees.forEach(y => allEmployees.pop(y));
    $.ajax({
        url: urlStandard+"/employees/DeleteEmployees?idsString=" + JSON.stringify(ids),
        type: 'DELETE',
        contentType: 'application/json',
        success: function (res) {
            var button = $("#trashButton").dxButton("instance");
                button.option('disabled', true);  
            $("#DeleteConfirmMany").dxPopup("instance").hide();
            $("#EmployeesGridContainer").dxDataGrid("getDataSource").reload();
        }
    })
}
function ClosePopUpDelete() {
    $("#DeleteConfirm").dxPopup("instance").hide();
}
function ClosePopUpDeleteMany() {
    $("#DeleteConfirmMany").dxPopup("instance").hide();
}
function ToolbarPreparing(e) {
    e.toolbarOptions.items.unshift(
        {
        location: "before",
        widget: "dxTextBox",
        options: {
            value: "Employees",
            readOnly: true,
            stylingMode: "filled",
            width:87
        }},
        {
            location: "after",
            widget: "dxButton",
            options: {
                icon: "trash",
                disabled: true,
                onClick: function () {
                    $("#DeleteConfirmMany").dxPopup("instance").show();
                },
                elementAttr: {
                    id: "trashButton",
                }
            }
        },
        {
            location: "after",
            widget: "dxButton",
            options: {
                icon: "refresh",
                onClick: function () {
                    $("#EmployeesGridContainer").dxDataGrid("getDataSource").reload();
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
                    InitialValidationNewEmployee();
                }
            }
        },
        {
            location: "after",
            widget: "dxButton",
            options: {
                icon: "inserttable",
                onClick: function () {
                    $("#ImportEmployeesPopUp").dxPopup("show");
                }
            }
        }
    );
}
function InitialValidationNewEmployee() {
    $("#NewEmployeePopUp").dxPopup("show");
    $("#nameNew").dxTextBox().dxValidator({
        validationGroup: "NewPopUp"
    });
    $("#surnameNew").dxTextBox().dxValidator({
        validationGroup: "NewPopUp"
    });
    $("#hiringDateNew").dxDateBox().dxValidator({
        validationGroup: "NewPopUp"
    });
    $("#SavePopUpNew").dxButton({
        validationGroup: "NewPopUp"
    });
    $('#SkillsGridContainer').dxDataGrid({
        columns: [
            {
                dataField: "Name",
                validationRules: [{ type: "required", message: "Field is required" },
                    {
                        type: "stringLength",
                        min: 1,
                        max: 64,
                        message: "Maximum length 64 characters"
                    },
                    {
                    type: "custom",
                    validationCallback: function (e) { return SkillNameValidationCallBack(e, allSkills, "#SkillsGridContainer") },
                    message: "Name already exists"
                }]
            },
            {
                dataField: "Description",
                validationRules: [{ type: "required", message: "Field is required" },
                    {
                        type: "stringLength",
                        min: 1,
                        max: 256,
                        message: "Maximum length 256 characters"
                    },
                    {
                    type: "custom",
                    validationCallback: function (e) {
                        return SkillDescriptionValidationCallBack(e, allSkills, "#SkillsGridContainer");
                    },
                    message: "Description already exists"
                }],
                caption: "Description"
            }
        ]
    })
    $("#SkillsGridContainer").dxDataGrid("getDataSource").reload();
}
function SkillNameValidationCallBack(e,matrix,grid) {
    var newName = e.value;
    var counter = 0;
    if ($(grid).dxDataGrid("instance") != undefined) {
        var itemsInGrid = matrix;
        var modifiedItems = $(grid).dxDataGrid("instance").option("editing.changes");
        var combinedMatrix = [];
        itemsInGrid.forEach(t => {
            var push = true;
            modifiedItems.forEach(p => { if (p.type == 'remove') { push = false; } })
            if (push)
                combinedMatrix.push(t)
        })
        modifiedItems.forEach(p => { if (p.type == 'insert') { combinedMatrix.push(p.data) } })
        combinedMatrix.forEach(r => {
            if (r.Name == newName)
                counter++
        })
        return counter <= 1;
    }
}
function SkillDescriptionValidationCallBack(e,matrix,grid) {
    var newDescription = e.value;
    var counter = 0;
    if ($(grid).dxDataGrid("instance") != undefined) {
        var itemsInGrid = matrix;
        var modifiedItems = $(grid).dxDataGrid("instance").option("editing.changes");
        var combinedMatrix = [];
        itemsInGrid.forEach(t => {
            var push = true;
            modifiedItems.forEach(p => { if (p.type == 'remove') { push = false; } })
            if (push)
                combinedMatrix.push(t)
        })
        modifiedItems.forEach(p => { if (p.type == 'insert') { combinedMatrix.push(p.data) } })
        combinedMatrix.forEach(r => {
            if (r.Description == newDescription)
                counter++
        })
        return counter <= 1;
    }
}
function ToolbarPreparingNew(e) {
   var toolbarItems = e.toolbarOptions.items;
   $.each(toolbarItems, function (_, item) {
       if (item.name == "saveButton") {
           item.visible = false;
       }
       if (item.name == 'revertButton') {
           item.options.onClick = function (args) {
               e.component.cancelEditData();
               e.component.refresh();
           }
       }
   });
}
function SaveNew(e) {
    ValidateFormNew();
    $("#SkillsGridContainer").dxDataGrid("instance").getController("validating").validate(true);
    var result = e.validationGroup.validate();
    if (result.isValid && allowSaveNew) {
        var newSkills = [];
        ($("#SkillsGridContainer").dxDataGrid("instance").option("editing.changes")).forEach(x => {
            var skill = new Skill(0, x.data.Name, x.data.Description, false, new Date(), null);
            allSkills.push(skill);
            newSkills.push(skill);
        })
        var skills = $("#SkillsGridContainer").dxDataGrid("instance").getSelectedRowsData();
        skills.forEach(s => newSkills.push(s));
        var employee = new Employee(0, $('#nameNew input').val(), $('#surnameNew input').val(), $("#hiringDateNew input").val(), newSkills, new Date(), "Creation");
        allEmployees.push(employee);
        $.ajax({
            url: urlStandard + "/employees/CreateEmployee",
            type: 'POST',
            data: JSON.stringify(JSON.stringify(employee)),
            contentType: 'application/json',
            success: function (res) {
                CancelNew();
                $("#NewEmployeePopUp").dxPopup("hide");
                $("#EmployeesGridContainer").dxDataGrid("getDataSource").reload();
            }
        })
    }
}
function HidingNew(){
    $('#nameNew input').val('');
    $('#surnameNew input').val('');
    $('#hiringDateNew input').val('');
    $('#hiringDateNew').dxValidator('instance').reset();
    $('#nameNew').dxValidator('instance').reset();
    $('#surnameNew').dxValidator('instance').reset();
    var grid = $("#SkillsGridContainer").dxDataGrid("instance");
    if (grid != undefined) {
        grid.option('paging.pageIndex', 0);
        grid.clearSelection();
        grid.option("focusedRowIndex", -1);
        grid.cancelEditData();
        grid.refresh();
    }
}
function CancelNew() {
    HidingNew();
    $('#NewEmployeePopUp').dxPopup("hide");
}
function DataInSkillsGrid(key) {
    skillsInEditpopUp = [];
    $.ajax({
        url: urlStandard + "/employees/GetSkillsForEmployee?id=" + key,
        type: 'GET',
        success: function (res) {
            res.Skillset.forEach(i => {
                skillsInEditpopUp.push(i);
            });
            $('#SkillsGridContainerEdit').dxDataGrid({
                dataSource: skillsInEditpopUp,
                paging: {
                    enabled: true,
                    pageSize: 5
                },
                keyExpr: "Id",
                showBorders: true,
                allowColumnResizing: true,
                allowColumnReordering: true,
                allowColumnResizing: true,
                columnMinWidth:50,
                searchPanel: { visible: true },
                focusedRowEnabled: true,
                editing: {
                    mode: "batch",
                    allowUpdating: false,
                    allowAdding: false,
                    allowDeleting: false
                },
                columns: [
                    {
                        dataField: "Name",
                        width: 200,
                        caption: "Name",
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
                                return SkillNameValidationCallBack(e, skillsInEditpopUp,"#SkillsGridContainerEdit");
                            },
                            message: "Name already exists"
                        }],
                        calculateDisplayValue: "Name",
                        editorOptions: {
                            showClearButton: true,
                            acceptCustomValue: true,
                            onCustomItemCreating: function (args) {
                                args.customItem = { Name: args.text };
                            }
                        },
                        lookup: {
                            dataSource: allSkills,
                            displayExpr: "Name",
                            valueExpr: "Name",
                            searchEnabled:true
                        }
                    },
                    {
                        dataField: "Description",
                        validationRules: [{ type: "required", message: "Field is required" },
                            {
                                type: "stringLength",
                                min: 1,
                                max: 256,
                                message: "Maximum length 256 characters"
                            },
                            {
                            type: "custom",
                            validationCallback: function (e) {
                                return SkillDescriptionValidationCallBack(e, skillsInEditpopUp, "#SkillsGridContainerEdit");
                            },
                            message: "Description already exists"
                        }],
                        caption: "Description"
                    },
                    {
                        dataField: "Id",
                        visible: false
                    }
                ],
                onToolbarPreparing: function (e) {
                    var toolbarItems = e.toolbarOptions.items;
                    $.each(toolbarItems, function (_, item) {
                        if (item.name == "saveButton") {
                            item.visible = false;
                        }
                        if (item.name == 'revertButton') {
                            item.options.onClick = function (args) {
                                e.component.cancelEditData();
                                e.component.refresh();
                            }
                        }
                    });
                },
                onRowValidating: function (e) {
                    if (!e.isValid) {
                        allowSave = false;
                    }
                    else {
                        allowSave = true;
                    }
                },
                onEditorPreparing: function (e) {
                    var  rowIndex = e.row && e.row.rowIndex;
                    if (e.dataField === "Name") {
                        var onValueChanged = e.editorOptions.onValueChanged;
                        e.editorOptions.onValueChanged = function (_e) {
                            onValueChanged.call(this, _e);
                            var skillExists;
                            for (var x = 0; x < allSkills.length; x++) {
                                if (allSkills[x].Name == e.row.cells[rowIndex].value) {
                                    e.component.cellValue(rowIndex, "Description", allSkills[x].Description);
                                    e.component.cellValue(rowIndex, "Id", allSkills[x].Id);
                                    skillExists = true;
                                }
                            }
                            if (!skillExists) {
                                e.component.cellValue(rowIndex, "Id", 0);
                            }
                        }
                    }
                }
            })
        }
    });
}
function SelectionChanged(){
    ids = $('#EmployeesGridContainer').dxDataGrid('instance').getSelectedRowKeys();
    var button = $("#trashButton").dxButton("instance");
    if (ids.length != 0) 
      button.option('disabled', false);  
    else
      button.option('disabled', true);  
}
function PageSize() {
    var mainHeight = $('main')[0].offsetHeight;
    var toolbarAndPagingHeight = 101;
    var headerRowHeight = 33;
    var gridHeight = mainHeight * 0.9 - toolbarAndPagingHeight - headerRowHeight;
    var pages = gridHeight / 33;
    return pages | 0;
}
function CalculateDisplayValueSkills(data) {
        var skills = data.Skillset;
        var skillsString = '';
        if (skills.length > 0) {
            skillsString = skills[0].Name;
            skills.forEach((x, i) => {               
                if (i != 0)
                    skillsString = skillsString + ", " + x.Name;
            });
        }
    return skillsString;
}
function HeaderFilterDataSource(data) {
    data.dataSource.postProcess = function (results) {
        allSkillsOfEmployees = [];
        results = [];
        allEmployees.forEach(z => {
            z.Skillset.forEach(i => {
                var add = true;
                allSkillsOfEmployees.forEach(s => {
                    if (s.Name == i.Name)
                        add = false;
                });
                if (add)
                    allSkillsOfEmployees.push(i);
            })
        })
        allSkillsOfEmployees.push('');
        allSkillsOfEmployees.forEach(x => {
            if (x.Name != '' && x.Description != '') {
                results.push({
                    text: x.Name,
                    value: x.Name
                });
            }
        });
        return results;
    };
}
function CalculateFilterExpression(value, selectedFilterOperations, target) {
    return [CalculateFilterExpressionSecond, 'contains', value];
}
function CalculateFilterExpressionSecond(rowData) {
    var skills = rowData.Skillset;
    var skillsString = "";
    if (skills.length > 0) {
        var skillsString = skills[0].Name;
        skills.forEach(x => {
            skillsString = skillsString + ', ' + x.Name;
        })
    }
    return skillsString;
}
function EmployeeNameValidationCallBack(e, allEmployees, field) {
    if (startingFieldForEmployeeValidation == '')
        startingFieldForEmployeeValidation = 'name';
    if (startingFieldForEmployeeValidation == 'name') {
        var surname = $('#surname' + field + ' input').val();
        $("#surname" + field).dxValidator('instance').reset();
        $("#surname" + field).dxTextBox({ value: surname });
        var hiringDate = $('#hiringDate' + field + ' input').val();
        $("#hiringDate" + field).dxValidator('instance').reset();
        $("#hiringDate" + field).dxDateBox({ value: hiringDate });
    }
    var newName;
    if (e != null)
        newName = e.value;
    else
        newName = $('#name' + field + ' input').val();
    var id = $('#id input').val();
    var itemsInGrid = allEmployees;
    var count = 0;
    if (field == '') {
        itemsInGrid.forEach(i => {
            if (i.Name == newName && i.Surname == $("#surname" + field + " input").val() && i.HiringDate.indexOf($("#hiringDate" + field + " input").val()) ==0  && i.Id != id)
                count++;
        })
    }
    else {
        itemsInGrid.forEach(i => {
            if (i.Name == newName && i.Surname == $("#surname" + field + " input").val() && i.HiringDate.indexOf($("#hiringDate" + field + " input").val()) == 0)
                count++;
        })
    }
    if (startingFieldForEmployeeValidation == 'name')
        startingFieldForEmployeeValidation = '';
    return count == 0;
}
function EmployeeSurnameValidationCallBack(e, allEmployees, field) {
    if (startingFieldForEmployeeValidation == '')
        startingFieldForEmployeeValidation = 'surname';
    if (startingFieldForEmployeeValidation == 'surname') {
        var name = $('#name' + field + ' input').val();
        $("#name" + field).dxValidator('instance').reset();
        $("#name" + field).dxTextBox({ value: name });
        var hiringDate = $('#hiringDate' + field + ' input').val();
        $("#hiringDate" + field).dxValidator('instance').reset();
        $("#hiringDate" + field).dxDateBox({ value: hiringDate });
    }
    var newSurname;
    if (e != null)
        newSurname = e.value;
    else 
        newSurname = $('#surname' + field + ' input').val();
    var id = $('#id input').val();
    var itemsInGrid = allEmployees;
    var count = 0;
    if (field == '') {
        itemsInGrid.forEach(i => {
            if (i.Surname == newSurname && i.Name == $('#name' + field + ' input').val() && i.HiringDate.indexOf($("#hiringDate" + field + " input").val()) == 0 && i.Id != id)
                count++;
        })
    }
    else {
        itemsInGrid.forEach(i => {
            if (i.Surname == newSurname && i.Name == $('#name' + field + ' input').val() && i.HiringDate.indexOf($("#hiringDate" + field + " input").val()) == 0)
                count++;
        })
    }
    if (startingFieldForEmployeeValidation == 'surname')
        startingFieldForEmployeeValidation = '';
    return count == 0;
}
function EmployeeHiringDateValidationCallBack(e, allEmployees, field) {
    if (startingFieldForEmployeeValidation == '')
        startingFieldForEmployeeValidation = 'hiringDate';
    if (startingFieldForEmployeeValidation == 'hiringDate') {
        var name = $('#name' + field + ' input').val();
        $("#name" + field).dxValidator('instance').reset();
        $("#name" + field).dxTextBox({ value: name });
        var surname = $('#surname' + field + ' input').val();
        $("#surname" + field).dxValidator('instance').reset();
        $("#surname" + field).dxTextBox({ value: surname });
    }
    var newHiringDate;
    if (e != null)
        newHiringDate = e.value;
    else
        newHiringDate = $('#hiringDate' + field + ' input').val();
    var id = $('#id input').val();
    var itemsInGrid = allEmployees;
    var count = 0;
    if (field == '') {
        itemsInGrid.forEach(i => {
            if (i.HiringDate.indexOf(newHiringDate) == 0 && i.Name == $("#name" + field + " input").val() && i.Surname == $("#surname" + field + " input").val() && i.Id != id)
                count++;
        })
    }
    else {
        itemsInGrid.forEach(i => {
            if (i.HiringDate.indexOf(newHiringDate) == 0 && i.Name == $("#name" + field + " input").val() && i.Surname == $("#surname" + field + " input").val())
                count++;
        })
    }
    if (startingFieldForEmployeeValidation == 'hiringDate')
        startingFieldForEmployeeValidation = '';
    return count == 0;
}
function ValidateForm() {
    $("#name").dxTextBox({
        value: $('#name input').val()
    }).dxValidator({
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
                return EmployeeNameValidationCallBack(e, allEmployees, "");
            },
            message: "Combination Name, Surname, Hiring Date already exists"
        }]
    });
    $("#surname").dxTextBox({
        value: $('#surname input').val()
    })
        .dxValidator({
            validationGroup: "EditPopUp",
            validationRules: [{
                type: "required",
                message: "Field is required"
            },
            {
                type: "stringLength",
                min: 1,
                max: 128,
                message: "Maximum length 128 characters"
            },
            {
                type: "custom",
                validationCallback: function (e) {
                    surnameValidator = e.validator;
                    return EmployeeSurnameValidationCallBack(e, allEmployees, "");
                },
                message: "Combination Name, Surname, Hiring Date already exists"
            }]
        });
    $("#hiringDate").dxDateBox({
        value: $('#hiringDate input').val()
    }).dxValidator({
            validationGroup: "EditPopUp",
            validationRules: [
            {
                type: "custom",
                    validationCallback: function (e) {
                    return EmployeeHiringDateValidationCallBack(e, allEmployees, "");
                },
                message: "Combination Name, Surname, Hiring Date already exists"
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
                return EmployeeNameValidationCallBack(e, allEmployees, "New");
            },
            message: "Combination Name, Surname, Hiring Date already exists"
        }]
    });
    $("#surnameNew").dxTextBox({ value: $('#surnameNew input').val() }).dxValidator({
        validationGroup: "NewPopUp",
        validationRules: [{
            type: "required",
            message: "Field is required"
        },
        {
            type: "stringLength",
            min: 1,
            max: 128,
            message: "Maximum length 128 characters"
        },
        {
            type: "custom",
            validationCallback: function (e) {
                return EmployeeSurnameValidationCallBack(e, allEmployees, "New");
            },
            message: "Combination Name, Surname, Hiring Date already exists"
        }]
    });
    $("#hiringDateNew").dxDateBox({ value: $('#hiringDateNew input').val() }).dxValidator({
        validationGroup: "NewPopUp",
        validationRules: [{
            type: "required",
            message: "Field is required"
        },
        {
            type: "custom",
            validationCallback: function (e) {
                return EmployeeHiringDateValidationCallBack(e, allEmployees, "New");
            },
            message: "Combination Name, Surname, Hiring Date already exists"
        }]
    });
    $("#SavePopUpNew").dxButton({
        validationGroup: "NewPopUp"
    });
}
function CellPrepared(e) {
    if (e.rowType === "data") {
        var $links = e.cellElement.find(".dx-link");
        if (e.row.data.hasOwnProperty("Id"))
            $links.filter(".dx-link-delete").remove();
    }  
}
function RowValidating(e) {
    if (!e.isValid) {
        allowSaveNew = false;
    }
    else {
        allowSaveNew = true;
    }
}
function Import() {
    var newEmployees = [];
    var selected = $("#ImportEmployeesGridContainer").dxDataGrid('instance').getSelectedRowsData();
    selected.forEach(s => {
        var exists = false;
        allEmployees.forEach(a => {
            if (a.Name == s.Name && a.Surname == s.Surname && a.HiringDate == s.HiringDate)
                exists = true;
        });
        if (!exists) {
            s.Id = 0;
            s.TypeOfSkillChange = "Creation";
            s.DateSkillChanged = new Date();
            newEmployees.push(s);
            allEmployees.push(s);
        }
    })
    $.ajax({
        url: urlStandard + "/employees/CreateEmployees",
        type: 'POST',
        data: JSON.stringify(JSON.stringify(newEmployees)),
        contentType: 'application/json',
        success: function (res) {
            $("#ImportEmployeesPopUp").dxPopup("hide");
            $("#EmployeesGridContainer").dxDataGrid("getDataSource").reload();
            RecalculateAllEmployees();
        }
    })
}
function HidingImport() {
    var grid = $("#ImportEmployeesGridContainer").dxDataGrid("instance");
    if (grid != undefined) {
        grid.option('paging.pageIndex', 0);
        grid.clearSelection();
        grid.option("focusedRowIndex", -1);
    }
}
function CancelImport() {
    HidingImport();
    $("#ImportEmployeesPopUp").dxPopup("hide");
}
function ToolbarPreparingImport(e) {
    var toolbarItems = e.toolbarOptions.items;
    $.each(toolbarItems, function (_, item) {
        if (item.name == "saveButton") {
            item.visible = false;
        }
    })
}
function ViewChanges() {
    $.ajax({
        url: urlStandard + '/employees/GetChangesForEmployee?id=' + $('#id input').val(),
        type: 'GET',
        success: function (res) {
            var auditdata = res;
            DataInHistoryGrid(auditdata);
            $("#HistoryPopUp").dxPopup("instance").show();
        }
    });
}
function DataInHistoryGrid(matrix) {
    $(document).ready(function () {
        $('#HistoryGridContainer').dxDataGrid({
            dataSource: matrix,
            paging: {
                enabled: true,
                pageSize: 8
            },
            keyExpr: "DateTime",
            showBorders: true,
            allowColumnResizing: true,
            allowColumnReordering: true,
            allowColumnResizing: true,
            columnMinWidth: 50,
            searchPanel: { visible: true },
            focusedRowEnabled: true,
            columns: [
                {
                    dataField: "DateTime",
                    width: 150,
                    caption: "Date and Time",
                    calculateDisplayValue: function (e) {
                        var date = new Date(e.DateTime);
                        var year = date.getFullYear();
                        var day = date.getDate();
                        if (day.toString().length == 1)
                            day = '0' + day;
                        var month = date.getMonth() + 1;
                        if (month.toString().length == 1)
                            month = '0' + month;
                        var hours = date.getHours() +3;
                        if (hours.toString().length == 1)
                            hours = '0' + hours;
                        var minutes = date.getMinutes();
                        if (minutes.toString().length == 1)
                            minutes = '0' + minutes;
                        var seconds = date.getSeconds();
                        if (seconds.toString().length == 1)
                            seconds = '0' + seconds;
                        return day + "/" + month + "/" + year + " " + hours + ":" + minutes + ":" + seconds;
                    }

                },
                {
                    dataField: "Comment",
                    caption: "Changes"
                }
            ],
            onToolbarPreparing: function (e) {
                var toolbarItems = e.toolbarOptions.items;
                $.each(toolbarItems, function (_, item) {
                    if (item.name == "saveButton") {
                        item.visible = false;
                    }
                });
            }
        })
    });
}
function CancelHistory() {
    $("#HistoryPopUp").dxPopup("hide");
}
