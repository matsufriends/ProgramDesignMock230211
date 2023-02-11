using ProgramDesignMock230211.Grids.Views;

namespace ProgramDesignMock230211.Grids
{
    /// <summary>
    /// GridのModelとViewを仲介するPresenter
    /// </summary>
    public sealed class GridPresenter
    {
        /// <summary>
        /// Gridのロジック部分
        /// </summary>
        private readonly GridModel _gridModel;

        /// <summary>
        /// Gridの可視化部分
        /// </summary>
        private readonly IGridView _gridViewMono;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model">Gridのロジック部分</param>
        /// <param name="gridView">Gridの可視化部分</param>
        public GridPresenter(GridModel model, IGridView gridView)
        {
            _gridModel = model;
            _gridViewMono = gridView;
        }
    }
}
