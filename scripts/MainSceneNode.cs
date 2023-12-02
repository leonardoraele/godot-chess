using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Raele;

public partial class MainSceneNode : Node2D
{
	[Export] private MapGridNode GridNode;
	[Export] private Label UserMessageNode;
	[Export] private BoardSetting Setting;

	private MainSceneController Controller = new MainSceneController();

	public override void _Ready() {
		this.UserMessageNode.Hide();
		this.Controller.UnitMovedEvent += this.OnUnitMoved;
		this.Controller.SelectionChangedEvent += this.OnSelectionChanged;
		this.Controller.UnitSelectionRequestEvent += this.OnUnitSelectionRequested;
		this.Controller.TileSelectionRequestEvent += this.OnTileSelectionRequested;
		this.Controller.Setup(this.Setting);
		this.GridNode.Refresh(this.Controller.Grid);
		this.Controller.Begin();
	}

	private async Task OnUnitMoved(MainSceneController.UnitMovedEventData eventData) {
		this.GridNode.Refresh(this.Controller.Grid);
	}

	private async Task OnSelectionChanged(UnitInfo? unit) {
		// TODO
	}

	private async Task<UnitInfo> OnUnitSelectionRequested(MainSceneController.UnitSelectionRequest request) {
		this.UserMessageNode.Text = "Please select a unit.";
		this.UserMessageNode.Show();
		UnitInfo? unit = null;
		while (unit == null) {
			Vector2I selectedPosition = await this.GridNode.WaitForSignal<Vector2I>(MapGridNode.SignalName.TileClicked);
			if (this.Controller.Grid.GetUnitAtPosition(selectedPosition, out unit) && (request.selectionCriteria?.Invoke(unit) ?? true)) {
				// TODO
				// this.Soundboard.PlaySE("accept");
			} else {
				// TODO
				// this.Soundboard.PlaySE("reject");
			}
		}
		this.UserMessageNode.Hide();
		return unit;
	}

	private async Task<Vector2I> OnTileSelectionRequested(MainSceneController.TileSelectionRequest request) {
		this.UserMessageNode.Text = "Please select a target.";
		this.UserMessageNode.Show();
		try {
			while (true) {
				Vector2I selectedPosition = await this.GridNode.WaitForSignal<Vector2I>(MapGridNode.SignalName.TileClicked);
				if (request.ValidPositions.Contains(selectedPosition)) {
					// this.Soundboard.PlaySE("accept"); // TODO
					return selectedPosition;
				} else {
					// this.Soundboard.PlaySE("reject"); // TODO
				}
			}
		} finally {
			this.UserMessageNode.Hide();
		}
	}
}