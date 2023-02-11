using System;
using ProgramDesignMock230211.Grids.Views;
using UniRx;
using UnityEngine;

namespace ProgramDesignMock230211.Grids
{
    /// <summary>
    ///     GridのModelとViewを仲介するPresenter
    /// </summary>
    public sealed class GridPresenter : IDisposable
    {
        /// <summary>
        ///     Gridのロジック部分
        /// </summary>
        private readonly GridModel _gridModel;

        /// <summary>
        ///     Gridの可視化部分
        /// </summary>
        private readonly IGridView _gridView;

        /// <summary>
        ///     MVP関連の購読一括解除用のCompositeDisposable
        /// </summary>
        private readonly CompositeDisposable _compositeDisposable = new();

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="gridSize">Gridの盤面サイズ</param>
        /// <param name="gridView">Gridの可視化部分</param>
        public GridPresenter(Vector2Int gridSize, IGridView gridView)
        {
            _gridModel = new GridModel(gridSize);
            _gridView = gridView;
            _gridModel.OnUpdatePlaceablePos.Subscribe(gridView.DisplayPlaceablePos).AddTo(_compositeDisposable);
            _gridModel.OnUpdatePiece.Subscribe(gridView.UpdatePiece).AddTo(_compositeDisposable);
            _gridView.OnGridSelected.Subscribe(x =>
                {
                    if (_gridModel.TryPlacePiece(x) == false)
                    {
                        return;
                    }

                    if (_gridModel.TryChangeTurn() == false)
                    {
                        Debug.Log("ゲーム終了");
                    }
                })
                .AddTo(_compositeDisposable);
            _gridView.Initialize(gridSize);
            _gridModel.InitializeBoard();
        }

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}
