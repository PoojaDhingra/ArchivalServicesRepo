<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebService.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
       <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.3/jquery.min.js"></script>
    <script type="text/javascript">
        function UploadSingleFile() {             
            var caseId = $("#caseNumber").val(); 
             var fileInput = $("#singleFileSelectorInput");          
             for (var i = 0; i < fileInput[0].files.length; i++) {
                 var file = fileInput[0].files[i]; 
                 ProcessUploadSingle(caseId, file);
             }
         }


        function ProcessUploadSingle(caseId, fileInput) {
            var reader = new FileReader();
            reader.onload = function (result) {
                var fileData = '';

                var byteArray = new Uint8Array(result.target.result)
                for (var i = 0; i < byteArray.byteLength; i++) {
                    fileData += String.fromCharCode(byteArray[i])
                }
                                      
                PerformUploadSingle(caseId, fileInput.name, fileData);

            };
            reader.readAsArrayBuffer(fileInput);
        }

        function PerformUploadSingle(caseId, fileName, fileData) {
            alert(fileData);
            alert(fileName);
            alert(caseId);
            $.ajax({
                url: 'Default.aspx/UploadFile',
                type: 'POST',
                data: JSON.stringify({
                    fileName: fileName,
                    fileData: fileData,
                    caseId: caseId
                }),
                contentType: "application/json; charset=UTF-8",
                success: OnSuccess,
                error: function (err) {
                    alert("error " + JSON.stringify(err));
                }
            });
        }

      
function OnSuccess(response) {
    alert(response.d);
}
</script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
         <div style="border:2px solid black;padding:8px;margin:auto 0; width:30%; background-color:khaki;">
        <div>
            Case Number
            <input type="text" id="caseNumber" />           
        </div>
        <br />
        <div>
            Select a single file
            <input type="file" id="singleFileSelectorInput" />
        </div>
        <br />
        <div>
            <input type="button" onclick="UploadSingleFile()" value="Upload Single Document" />
        </div>
    </div>

    
    </div>

        <div>
Your Name :
<%--<asp:TextBox ID="txtUserName" runat="server"></asp:TextBox>
<input id="btnGetTime" type="button" value="Show Current Time"
    onclick = "ShowCurrentTime()" />  function ShowCurrentTime() {
            $.ajax({
                type: "POST",
                url: "Default.aspx/GetCurrentTime",
                data: '{name: "' + $("#<%=txtUserName.ClientID%>")[0].value + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccess,
        failure: function (response) {
            alert(response.d);
        }
    });
}
</div>--%>
    </form>
</body>
</html>
