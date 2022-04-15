export const convertDate = isoString => {
    let dateArray = isoString.split(/\D+/);

    const convertedDate = new Date(Date.UTC(
        dateArray[0],
        --dateArray[1],
        dateArray[2],
        dateArray[3],
        dateArray[4],
        dateArray[5],
        dateArray[6],
    ))

    return new Date(convertedDate).toLocaleDateString("en-EU", {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: 'numeric',
        hour12: false,
        minute: 'numeric'
    }).replace(/\//g, '-');
}
