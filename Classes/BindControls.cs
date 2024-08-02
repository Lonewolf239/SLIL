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
        public Keys Medkit { get; set; }
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
            Medkit = BindControls["medkit"];
            Run = BindControls["run"];
        }
    }
}