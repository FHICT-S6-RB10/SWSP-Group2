export const sortMessagesByDate = messages => {
    messages.sort((a, b) => {
        const aDate = new Date(a.date);
        const bDate = new Date(b.date);

        if(aDate > bDate) {
            return -1;
        } else if (aDate < bDate) {
            return 1;
        } else {
            return 0;
        }
    });

    return messages;
}
