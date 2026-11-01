/// <summary>
/// ============================================================================
/// 檔案名稱: IBarrierController.cs
/// 所屬模組: Controllers/Interfaces
/// 責任描述: 規範場景通行阻擋物 (升降柵欄、通道滑動閘門等) 之開關與通行狀態契約 (DIP)。
/// 架構分層: Controllers / Interfaces
/// ============================================================================
/// </summary>
public interface IBarrierController
{
    bool IsOpen { get; }
    void Open();
    void Close();
}
