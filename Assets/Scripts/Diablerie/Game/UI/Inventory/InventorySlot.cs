﻿using Diablerie.Engine;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using Diablerie.Engine.UI;
using Diablerie.Engine.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Diablerie.Game.UI.Inventory
{
    public class InventorySlot : 
        MonoBehaviour,
        IPointerDownHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public int bodyLoc;
        public Item item;
        public Image placeholder;
        public Image highlighter;
        public Image itemImage;
        private bool pointerOver = false;

        private bool CanEquip(Item item)
        {
            return PlayerController.instance.equip.CanEquip(item, bodyLoc);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerOver = true;
            var mouseItem = PlayerController.instance.mouseItem;
            if (mouseItem != null && !CanEquip(mouseItem))
                highlighter.color = Colors.InvItemHighlightForbid;
            else
                highlighter.color = Colors.InvItemHighlight;
            highlighter.gameObject.SetActive(mouseItem != null || item != null);
        }

        private void ShowLabel()
        {
            var rect = Tools.RectTransformToScreenRect(GetComponent<RectTransform>());
            var pos = new Vector2(rect.center.x, rect.yMax);
            Ui.ShowScreenLabel(pos, item.GetDescription());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pointerOver = false;
            highlighter.gameObject.SetActive(false);
            Ui.HideScreenLabel();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var mouseItem = PlayerController.instance.mouseItem;
            if (mouseItem != null && !CanEquip(mouseItem))
            {
                string charClass = PlayerController.instance.charStat.info.classNameLower;
                AudioManager.instance.Play(charClass + "_impossible_1");
                return;
            }

            Item[] unequipped = PlayerController.instance.equip.Equip(mouseItem, bodyLoc);

            if (mouseItem != null)
                AudioManager.instance.Play(mouseItem.useSound);
            else if (unequipped[0] != null)
                AudioManager.instance.Play(SoundInfo.itemPickup);

            PlayerController.instance.mouseItem = unequipped[0];
            if (unequipped[1] != null)
                if (!PlayerController.instance.inventory.Put(unequipped[1]))
                    Pickup.Create(PlayerController.instance.character.transform.position, unequipped[1]);
        }

        private void OnDisable()
        {
            pointerOver = false;
            highlighter.gameObject.SetActive(false);
            Ui.HideScreenLabel();
        }
    
        private void Update()
        {
            if (!pointerOver)
                return;

            var mouseItem = PlayerController.instance.mouseItem;
            if (item != null && mouseItem == null)
            {
                ShowLabel();
            }
            else
            {
                Ui.HideScreenLabel();
            }
        }
    }
}
