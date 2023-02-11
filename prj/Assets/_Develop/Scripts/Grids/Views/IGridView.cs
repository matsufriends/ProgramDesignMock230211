namespace ProgramDesignMock230211.Grids.Views
{
    /// <summary>
    /// Gridを可視化するViewのInterface
    /// </summary>
    public interface IGridView
    {
        /// <summary>
        /// 配置可能な座標を可視化する
        /// </summary>
        /// <param name="placeablePosInfo">配置可能な位置情報群</param>
        void DisplayPlaceablePos(PlaceablePosInfo placeablePosInfo);
    }
}
