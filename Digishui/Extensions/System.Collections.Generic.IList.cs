using System.ComponentModel;
using System.Data;

//=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
namespace Digishui.Extensions
{
  //===========================================================================================================================
  /// <summary>
  ///   System.Collections.Generic.IList Extensions
  /// </summary>
  public static partial class Extensions
  {
    //-------------------------------------------------------------------------------------------------------------------------
    public static DataTable ConvertToDataTable<T>(this IList<T> data)
    {
      PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(typeof(T));

      DataTable dataTable = new();

      foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollection)
      {
        dataTable.Columns.Add(
          propertyDescriptor.Name,
          Nullable.GetUnderlyingType(propertyDescriptor.PropertyType) ?? propertyDescriptor.PropertyType
        );
      }

      foreach (T item in data)
      {
        DataRow dataRow = dataTable.NewRow();
        foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollection)
        {
          dataRow[propertyDescriptor.Name] = propertyDescriptor.GetValue(item) ?? DBNull.Value;
        }

        dataTable.Rows.Add(dataRow);
      }

      return dataTable;
    }
  }
}