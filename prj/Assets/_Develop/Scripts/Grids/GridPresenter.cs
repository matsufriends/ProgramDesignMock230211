using System;
using ProgramDesignMock230211.Grids.Views;
using UniRx;

namespace ProgramDesignMock230211.Grids
{
    /// <summary>
    /// GridのModelとViewを仲介するPresenter
    /// </summary>
    public sealed class GridPresenter : IDisposable
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
        /// MVP関連の購読一括解除用のCompositeDisposable
        /// </summary>
        private readonly CompositeDisposable _compositeDisposable = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model">Gridのロジック部分</param>
        /// <param name="gridView">Gridの可視化部分</param>
        public GridPresenter(GridModel model, IGridView gridView)
        {
            _gridModel = model;
            _gridViewMono = gridView;
            //TODO: 購読処理
            _gridModel.OnUpdatePlaceablePos.Subscribe(gridView.DisplayPlaceablePos).AddTo(_compositeDisposable);
            _gridModel.InitializeBoard();
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}
