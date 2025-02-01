import * as ElementPlusIconsVue from '@element-plus/icons-vue';

export const getIcon = (name: string) => {
    // Возвращаем компонент иконки по её имени
    return icons().get(name);
}

const icons = (): Map<string, Component> => {
    let icons: Map<string,  any> = new Map();
    for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
        icons.set(key, component);
    }
    return icons
};