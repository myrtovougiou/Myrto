function Exporting(e) {
    var js_script = $('script[src*=export]');
    var fileNameParam = js_script.attr('fileName');

    var workbook = new ExcelJS.Workbook();
    var worksheet = workbook.addWorksheet('Main WorkSheet');

    DevExpress.excelExporter.exportDataGrid({
        component: e.component,
        worksheet: worksheet,
        autoFilterEnabled: true
    }).then(function () {
        // https://github.com/exceljs/exceljs#writing-xlsx
        workbook.xlsx.writeBuffer().then(function (buffer) {
            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileNameParam);
        });
    });
    e.cancel = true;
}