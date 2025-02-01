import type {MenuItem} from "~/models/MenuItem";

export const MenuItems: MenuItem[] = [
    { id: "1", name: "Posts", iconName: "Notebook", route: "/" },
    { id: "2", name: "Analytics", iconName: "Histogram", route: "stats" },
    { id: "3", name: "Settings", iconName: "Setting", route: "settings" },
    { id: "4", name: "Exit", iconName: "ArrowLeft", route: "signout" },
]