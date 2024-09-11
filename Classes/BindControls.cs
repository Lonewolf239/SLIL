using System.Collections.Generic;
using System.Windows.Forms;

namespace SLIL.Classes
{
    public class BindControls
    {
        public Keys Screenshot { get; set; }
        public Keys Reloading { get; set; }
        public Keys Forward { get; set; }
        public Keys Back { get; set; }
        public Keys Left { get; set; }
        public Keys Right { get; set; }
        public Keys Interaction_0 { get; set; }
        public Keys Interaction_1 { get; set; }
        public Keys Show_map_0 { get; set; }
        public Keys Show_map_1 { get; set; }
        public Keys Flashlight { get; set; }
        public Keys Item { get; set; }
        public Keys Select_item { get; set; }
        public Keys Run { get; set; }

        public BindControls(Dictionary<string, Keys> BindControls)
        {
            Screenshot = BindControls["screenshot"];
            Reloading = BindControls["reloading"];
            Forward = BindControls["forward"];
            Back = BindControls["back"];
            Left = BindControls["left"];
            Right = BindControls["right"];
            Interaction_0 = BindControls["interaction_0"];
            Interaction_1 = BindControls["interaction_1"];
            Show_map_0 = BindControls["show_map_0"];
            Show_map_1 = BindControls["show_map_1"];
            Flashlight = BindControls["flashlight"];
            Item = BindControls["item"];
            Select_item = BindControls["select_item"];
            Run = BindControls["run"];
        }

        public Keys GetKey(string name_properties)
        {
            switch (name_properties.ToLower())
            {
                case "screenshot": return Screenshot;
                case "reloading": return Reloading;
                case "forward": return Forward;
                case "back": return Back;
                case "left": return Left;
                case "right": return Right;
                case "interaction_0": return Interaction_0;
                case "interaction_1": return Interaction_1;
                case "show_map_0": return Show_map_0;
                case "show_map_1": return Show_map_1;
                case "flashlight": return Flashlight;
                case "item": return Item;
                case "select_item": return Select_item;
                case "run": return Run;
                default: return Keys.None;
            }
        }

        public bool ExistKey(string name_properties)
        {
            switch (name_properties.ToLower())
            {
                case "screenshot":
                case "reloading":
                case "forward":
                case "back":
                case "left":
                case "right":
                case "interaction_0":
                case "interaction_1":
                case "show_map_0":
                case "show_map_1":
                case "flashlight":
                case "item":
                case "select_item":
                case "run": return true;
                default: return false;
            }
        }
    }
}