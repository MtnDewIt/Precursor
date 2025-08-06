// Configuration (This is kinda bad :/)
const ROOT_FILE_PATH = 'Reports/TagDefinitions/Reports.json';

// State
let database = null;
let currentBuild = null;
let currentFile = null;
let currentGroup = null;
let currentTag = null;

// DOM elements
const statusDiv = document.getElementById('status');
const buildsDiv = document.getElementById('builds');
const filesDiv = document.getElementById('files');
const groupsDiv = document.getElementById('groups');
const tagsDiv = document.getElementById('tags');
const errorsDiv = document.getElementById('errors');

// Auto-load database when page loads
document.addEventListener('DOMContentLoaded', loadDatabase);

async function loadDatabase() 
{
    try 
    {
        showStatus('Loading database...', 'info');

        // Load the root file from the static path
        const rootData = await loadJsonFile(ROOT_FILE_PATH);

        if (!rootData) 
        {
            throw new Error(`Could not load root file from ${ROOT_FILE_PATH}`);
        }

        if (!rootData.Builds || !Array.isArray(rootData.Builds)) 
        {
            throw new Error('Invalid root file format. Expected Builds array.');
        }

        // Get the base path from the root file path
        const rootPath = ROOT_FILE_PATH.substring(0, ROOT_FILE_PATH.lastIndexOf('/') + 1);

        // Load the complete database structure
        database = await loadCompleteDatabase(rootData);

        if (database && database.Builds && database.Builds.length > 0) 
        {
            showStatus(`Database loaded successfully! Found ${database.Builds.length} builds.`, 'success');
            displayBuilds();
            clearColumns(['files', 'groups', 'tags', 'errors']);
        } 
        else 
        {
            showStatus('No valid database structure found', 'error');
        }
    } 
    catch (error) 
    {
        console.error('Error loading database:', error);
        showStatus(`Error loading database: ${error.message}`, 'error');
    }
}

async function loadCompleteDatabase(rootData) 
{
    const result = 
    {
        Builds: []
    };

    for (const build of rootData.Builds) 
    {
        const buildEntry = 
        {
            Build: build.Build,
            Files: []
        };

        for (const filePath of build.Files) 
        {
            try 
            {
                const fileData = await loadJsonFile(`Reports/TagDefinitions/${filePath}`);

                if (!fileData) 
                    continue;

                const fileEntry = 
                {
                    FileName: fileData.FileName || filePath.split('/').pop(),
                    Groups: []
                };

                for (const groupPath of fileData.Groups || []) 
                {
                    try 
                    {
                        const groupData = await loadJsonFile(`Reports/TagDefinitions/${groupPath}`);

                        if (!groupData) 
                            continue;

                        const groupEntry = 
                        {
                            TagGroup: groupData.TagGroup,
                            GroupName: groupData.GroupName,
                            Tags: groupData.Tags || []
                        };

                        fileEntry.Groups.push(groupEntry);
                    } 
                    catch (error) 
                    {
                        console.warn(`Failed to load group file: ${groupPath}`, error);
                    }
                }
                buildEntry.Files.push(fileEntry);
            } 
            catch (error) 
            {
                console.warn(`Failed to load file: ${filePath}`, error);
            }
        }
        result.Builds.push(buildEntry);
    }
    return result;
}

async function loadJsonFile(relativePath) 
{
    try 
    {
        // For now, we'll show an error message since we can't access arbitrary files
        const response = await fetch(relativePath);

        if (!response.ok) 
        {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        return await response.json();
    } 
    catch (error) 
    {
        console.warn(`Could not load file ${relativePath}:`, error);
        return null;
    }
}

function readFileContent(file) 
{
    return new Promise((resolve, reject) => 
    {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result);
        reader.onerror = () => reject(reader.error);
        reader.readAsText(file);
    });
}

function showStatus(message, type) 
{
    statusDiv.textContent = message;
    statusDiv.className = `status ${type}`;
    statusDiv.style.display = 'block';

    if (type === 'success') 
    {
        setTimeout(() => { statusDiv.style.display = 'none'; }, 3000);
    }
}

function displayBuilds() 
{
    buildsDiv.innerHTML = '';

    if (!database || !database.Builds.length) 
    {
        buildsDiv.innerHTML = '<div class="empty-state">No builds available</div>';
        return;
    }

    database.Builds.forEach((build, index) => 
    {
        const item = document.createElement('div');
        item.className = 'list-item';
        item.innerHTML = `${build.Build}<span class="tag-count">(${build.Files.length} files)</span>`;

        item.addEventListener('click', () => selectBuild(index, item));
        buildsDiv.appendChild(item);
    });
}

function selectBuild(index, element) 
{
    clearSelection('builds');
    element.classList.add('selected');
    currentBuild = index;
    displayFiles();
    clearColumns(['groups', 'tags', 'errors']);
}

function displayFiles() 
{
    filesDiv.innerHTML = '';

    if (currentBuild === null || !database.Builds[currentBuild].Files.length) 
    {
        filesDiv.innerHTML = '<div class="empty-state">No files available</div>';
        return;
    }

    const files = database.Builds[currentBuild].Files;

    files.forEach((file, index) => 
    {
        const item = document.createElement('div');
        item.className = 'list-item';
        item.innerHTML = `${file.FileName}<span class="tag-count">(${file.Groups.length} groups)</span>`;

        item.addEventListener('click', () => selectFile(index, item));
        filesDiv.appendChild(item);
    });
}

function selectFile(index, element) 
{
    clearSelection('files');
    element.classList.add('selected');
    currentFile = index;
    displayGroups();
    clearColumns(['tags', 'errors']);
}

function displayGroups() 
{
    groupsDiv.innerHTML = '';

    if (currentFile === null || !database.Builds[currentBuild].Files[currentFile].Groups.length) 
    {
        groupsDiv.innerHTML = '<div class="empty-state">No groups available</div>';
        return;
    }

    const groups = database.Builds[currentBuild].Files[currentFile].Groups;

    groups.forEach((group, index) => 
    {
        const item = document.createElement('div');
        item.className = 'list-item';
        item.innerHTML = `<strong>${group.TagGroup}</strong><br><small>${group.GroupName}</small><span class="tag-count">(${group.Tags.length} tags)</span>`;

        item.addEventListener('click', () => selectGroup(index, item));
        groupsDiv.appendChild(item);
    });
}

function selectGroup(index, element) 
{
    clearSelection('groups');
    element.classList.add('selected');
    currentGroup = index;
    displayTags();
    clearColumns(['errors']);
}

function displayTags() 
{
    tagsDiv.innerHTML = '';

    if (currentGroup === null || !database.Builds[currentBuild].Files[currentFile].Groups[currentGroup].Tags.length) 
    {
        tagsDiv.innerHTML = '<div class="empty-state">No tags available</div>';
        return;
    }

    const tags = database.Builds[currentBuild].Files[currentFile].Groups[currentGroup].Tags;

    tags.forEach((tag, index) => 
    {
        const item = document.createElement('div');
        item.className = 'list-item';
        item.innerHTML = `${tag.TagName}<span class="tag-count">(${tag.Errors.length} errors)</span>`;

        item.addEventListener('click', () => selectTag(index, item));
        tagsDiv.appendChild(item);
    });
}

function selectTag(index, element) 
{
    clearSelection('tags');
    element.classList.add('selected');
    currentTag = index;
    displayErrors();
}

function displayErrors() 
{
    errorsDiv.innerHTML = '';

    if (currentTag === null) 
    {
        errorsDiv.innerHTML = '<div class="empty-state">No tag selected</div>';
        return;
    }

    const errors = database.Builds[currentBuild].Files[currentFile].Groups[currentGroup].Tags[currentTag].Errors;

    if (!errors.length) 
    {
        errorsDiv.innerHTML = '<div class="empty-state">No errors found ✓</div>';
        return;
    }

    errors.forEach(error => {
        const item = document.createElement('div');
        item.className = 'error-item';
        item.textContent = error;
        errorsDiv.appendChild(item);
    });
}

function clearSelection(columnType) 
{
    const container = document.getElementById(columnType);

    container.querySelectorAll('.list-item').forEach(item => 
    {
        item.classList.remove('selected');
    });
}

function clearColumns(columnTypes) 
{
    columnTypes.forEach(type => 
    {
        const container = document.getElementById(type);
        container.innerHTML = '<div class="empty-state">Select an item from the previous column</div>';
    });

    if (columnTypes.includes('files')) 
        currentFile = null;

    if (columnTypes.includes('groups')) 
        currentGroup = null;

    if (columnTypes.includes('tags')) 
        currentTag = null;
}

clearColumns(['builds', 'files', 'groups', 'tags', 'errors']);
document.getElementById('builds').innerHTML = '<div class="empty-state">Load your database to get started</div>';