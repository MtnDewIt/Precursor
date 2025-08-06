async function fetchJson(path) 
{
    const res = await fetch(path);
    return res.json();
}

async function buildTable() 
{
    const table = document.querySelector("#report-table tbody");

    const buildData = await fetchJson('Reports/TagDefinitions/Reports.json');

    for (const buildEntry of buildData.Builds) 
    {
        for (const filePath of buildEntry.Files) 
        {
            const fileData = await fetchJson(`Reports/TagDefinitions/${filePath}`);

            for (const groupPath of fileData.Groups) 
            {
                const groupData = await fetchJson(`Reports/TagDefinitions/${groupPath}`);

                for (const tag of groupData.Tags) 
                {
                    const errorList = tag.Errors.map(e => `<div class="error">${e}</div>`).join('');

                    const row = document.createElement("tr");

                    row.innerHTML = `
                        <td>${buildEntry.Build}</td>
                        <td>${fileData.FileName}</td>
                        <td>${groupData.TagGroup}</td>
                        <td>${tag.TagName}</td>
                        <td>${errorList}</td>`;
                        
                    table.appendChild(row);
                }
            }
        }
    }
}

buildTable().catch(err => 
{
    console.error(err);
    document.body.innerHTML = "<p>Failed to load report data.</p>";
});