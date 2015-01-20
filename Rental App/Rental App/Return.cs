﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Phidgets;
using Phidgets.Events;

namespace Rental_App
{
    public partial class Return : Form
    {
        public string RFID = "0a006f46d7";
        RFID reader;
        Item product;
        Items listOfItems = new Items("listOfItems");
        DatabaseConnection data = new DatabaseConnection();

        public Return()
        {
            InitializeComponent();

            reader = new RFID();
            reader.Attach += new AttachEventHandler(rfid_Attach);
            reader.Detach += new DetachEventHandler(rfid_Detach);
            reader.RFIDTag += new TagEventHandler(rfid_Tag);
            reader.RFIDTagLost += new TagEventHandler(rfid_TagLost);
            reader.Antenna = true;
            reader.open();

            foreach (Item i in data.LoadUserItem(RFID))
            {
                listOfItems.AddItem(i.Name, i.Price, i.Deposit, i.TotalLeft, i.iD);
                lbItems.Items.Add(i.Name);
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            int selected;

            if (this.lbItems.SelectedIndex >= 0)
            {
                selected = this.lbItems.SelectedIndex;

                string s = lbItems.Items[selected].ToString();
                //product = listOfItems.GetItems(selected);

                product = listOfItems.GetItems(s);

                lbItems.Items.RemoveAt(selected);

                if (data.Stocks(product.TotalLeft, "+", 1, product.iD) && data.deleteRent(product.iD, RFID))
                {
                    MessageBox.Show("Returned!");
                }

            }
        }

        // RFID
        void rfid_Tag(object sender, TagEventArgs e)
        {
            RFID = e.Tag;
            reader.LED = true;       // light on
            lblShow.Text = "Visitor: " + RFID;

            foreach (Item i in data.LoadUserItem(RFID))
            {
                listOfItems.AddItem(i.Name, i.Price, i.Deposit, i.TotalLeft, i.iD);
                lbItems.Items.Add(i.Name);
            }
 
        }

        void rfid_TagLost(object sender, TagEventArgs e)
        {

            reader.LED = false;      // light off
        }

        void rfid_Detach(object sender, DetachEventArgs e)
        {
            //lblAttached.Text = "Not Attached";
        }

        void rfid_Attach(object sender, AttachEventArgs e)
        {
            Phidgets.RFID phid = (Phidgets.RFID)sender;
            //lblAttached.Text = "Attached: " + phid.Name;

        }

        private void lbItems_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
