﻿using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using ReClassNET.UI;
using ReClassNET.Util;

namespace ReClassNET.Nodes
{
	[ContractClass(typeof(BaseTextNodeContract))]
	public abstract class BaseTextNode : BaseNode
	{
		public int Length { get; set; }

		/// <summary>Size of the node in bytes.</summary>
		public override int MemorySize => Length * CharacterSize;

		/// <summary>Size of one character in bytes.</summary>
		public abstract int CharacterSize { get; }

		public override void CopyFromNode(BaseNode node)
		{
			Length = node.MemorySize / CharacterSize;
		}

		protected Size DrawText(ViewInfo view, int x, int y, string type, int length, string text)
		{
			Contract.Requires(view != null);
			Contract.Requires(type != null);
			Contract.Requires(text != null);

			if (IsHidden)
			{
				return DrawHidden(view, x, y);
			}

			DrawInvalidMemoryIndicator(view, y);

			var origX = x;

			AddSelection(view, x, y, view.Font.Height);

			x += TextPadding;
			x = AddIcon(view, x, y, Icons.Text, HotSpot.NoneId, HotSpotType.None);
			x = AddAddressOffset(view, x, y);

			x = AddText(view, x, y, view.Settings.TypeColor, HotSpot.NoneId, type) + view.Font.Width;
			x = AddText(view, x, y, view.Settings.NameColor, HotSpot.NameId, Name);
			x = AddText(view, x, y, view.Settings.IndexColor, HotSpot.NoneId, "[");
			x = AddText(view, x, y, view.Settings.IndexColor, 0, length.ToString());
			x = AddText(view, x, y, view.Settings.IndexColor, HotSpot.NoneId, "]") + view.Font.Width;

			x = AddText(view, x, y, view.Settings.TextColor, HotSpot.NoneId, "= '");
			x = AddText(view, x, y, view.Settings.TextColor, HotSpot.NoneId, text.LimitLength(150));
			x = AddText(view, x, y, view.Settings.TextColor, HotSpot.NoneId, "'") + view.Font.Width;

			x = AddComment(view, x, y);

			AddTypeDrop(view, y);
			AddDelete(view, y);

			return new Size(x - origX, view.Font.Height);
		}

		public override int CalculateDrawnHeight(ViewInfo view)
		{
			return IsHidden ? HiddenHeight : view.Font.Height;
		}

		public override void Update(HotSpot spot)
		{
			base.Update(spot);

			if (spot.Id == 0)
			{
				if (int.TryParse(spot.Text, out var val) && val > 0)
				{
					Length = val;

					ParentNode.ChildHasChanged(this);
				}
			}
		}
	}

	[ContractClassFor(typeof(BaseTextNode))]
	internal abstract class BaseTextNodeContract : BaseTextNode
	{
		public override int CharacterSize
		{
			get
			{
				Contract.Ensures(Contract.Result<int>() > 0);

				throw new NotImplementedException();
			}
		}
	}
}
