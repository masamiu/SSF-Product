using System.Data;
using Suityou.Framework.Web.Common;
using Suityou.Framework.Web.DataManager;
using Suityou.Framework.Web.DataModel;
using Suityou.Framework.Web.DataManager;


// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

// SettingsManager Test
// GetDataInformation
/*string definitionFile = "D:\\ITESS\\20_Product\\suityou\\DataDefinition.json";
string dataID = "D1001";
DataInformation dataInfo = SettingsManager.GetDataInformation(definitionFile, dataID);
Console.WriteLine(dataInfo.ToString());
*/

// NormalDataManager Test
string applicationSettingFile = "D:\\ITESS\\20_Product\\suityou\\ApplicationSetting.json";
string dataDefinitionFile = "D:\\ITESS\\20_Product\\suityou\\DataDefinition.json";
string dataID = "D1001";
NormalDataManager dm = new NormalDataManager(0, applicationSettingFile, dataDefinitionFile, dataID);
Console.WriteLine(dm.GetInformation());

DataSet dsMain = dm.GetData();
foreach (DataRow row in dsMain.Tables[0].Rows)
{
    string dataContent = string.Empty;
    foreach (DataColumn col in row.Table.Columns)
    {
        if (String.IsNullOrEmpty(dataContent))
        {
            dataContent += col.ColumnName + "[" + row[col.ColumnName].ToString() + "]";
        }
        else
        {
            dataContent += ", " + col.ColumnName + "[" + row[col.ColumnName].ToString() + "]";
        }
    }
    Console.WriteLine(dataContent);
}

//DataSet dsSub = dm.GetAllSubData();
//foreach (DataTable dt in dsSub.Tables)
//{
//    Console.WriteLine("***************" + dt.TableName);

//    foreach (DataRow row in dt.Rows)
//    {
//        string dataContent = string.Empty;
//        foreach (DataColumn col in row.Table.Columns)
//        {
//            if (String.IsNullOrEmpty(dataContent))
//            {
//                dataContent += col.ColumnName + "[" + row[col.ColumnName].ToString() + "]";
//            }
//            else
//            {
//                dataContent += ", " + col.ColumnName + "[" + row[col.ColumnName].ToString() + "]";
//            }
//        }
//        Console.WriteLine(dataContent);
//    }
//}

DataTable targetTable = new DataTable();
DataColumn colDef = new DataColumn("COMPANY_ID", Type.GetType("System.Int32"));
targetTable.Columns.Add(colDef);
colDef = new DataColumn("COMPANY_CODE", Type.GetType("System.String"));
targetTable.Columns.Add(colDef);
colDef = new DataColumn("COMPANY_NAME", Type.GetType("System.String"));
targetTable.Columns.Add(colDef);
colDef = new DataColumn("STATUS", Type.GetType("System.Int32"));
targetTable.Columns.Add(colDef);

DataRow addRow = targetTable.NewRow();
addRow["COMPANY_ID"] = 3;
addRow["COMPANY_CODE"] = "0003";
addRow["COMPANY_NAME"] = "GMOインターネット";
addRow["STATUS"] = 0;

//dm.AddData(addRow);
//dm.UpdateData(addRow);
dm.DeleteData(addRow);

