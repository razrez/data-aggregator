export const formatDate = (date?: Date) => {
    if (!date) {
        return new Date();
    }

    return new Intl.DateTimeFormat('ru-RU', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
    }).format(date);
};