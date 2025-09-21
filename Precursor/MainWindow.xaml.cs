using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Precursor.Data;
using Precursor.ViewModels;

namespace Precursor
{
    public partial class MainWindow : Window
    {
        private string _reportsDirectory;
        private List<BuildViewModel> _builds;

        public MainWindow()
        {
            InitializeComponent();
            LoadBuilds();
        }

        private void LoadBuilds()
        {
            try
            {
                _reportsDirectory = Path.Combine(Environment.CurrentDirectory, "Reports", "TagDefinitions");

                var reportsFile = Path.Combine(_reportsDirectory, "Reports.json");

                if (!File.Exists(reportsFile))
                {
                    MessageBox.Show("Reports.json not found. Please run ValidateTagDefinitions first.");
                    return;
                }

                var json = File.ReadAllText(reportsFile);
                var data = JsonConvert.DeserializeObject<ReportData>(json);

                _builds = data.Builds.Select(b => new BuildViewModel
                {
                    Build = b.Build,
                    ErrorLevel = b.ErrorLevel,
                    FileErrorCount = b.FileErrorCount,
                    Files = b.Files,
                    ErrorInfo = $"Errors: {b.FileErrorCount}"
                }).ToList();

                BuildsList.ItemsSource = _builds;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading builds: {ex.Message}");
            }
        }

        private void BuildsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilesList.ItemsSource = null;
            GroupsList.ItemsSource = null;
            TagsList.ItemsSource = null;
            ErrorsList.ItemsSource = null;

            if (BuildsList.SelectedItem is BuildViewModel selectedBuild)
            {
                LoadFiles(selectedBuild);
            }
        }

        private void LoadFiles(BuildViewModel build)
        {
            try
            {
                var fileViewModels = new List<FileViewModel>();

                foreach (var file in build.Files)
                {
                    var filePath = Path.Combine(_reportsDirectory, file);

                    if (File.Exists(filePath))
                    {
                        var json = File.ReadAllText(filePath);
                        var fileData = JsonConvert.DeserializeObject<FileData>(json);

                        fileViewModels.Add(new FileViewModel
                        {
                            FilePath = file,
                            FileName = fileData.FileName,
                            ErrorLevel = fileData.ErrorLevel,
                            GroupErrorCount = fileData.GroupErrorCount,
                            Groups = fileData.Groups,
                            ErrorInfo = $"Errors: {fileData.GroupErrorCount}"
                        });
                    }
                }

                FilesList.ItemsSource = fileViewModels;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading files: {ex.Message}");
            }
        }

        private void FilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GroupsList.ItemsSource = null;
            TagsList.ItemsSource = null;
            ErrorsList.ItemsSource = null;

            if (FilesList.SelectedItem is FileViewModel selectedFile)
            {
                LoadGroups(selectedFile);
            }
        }

        private void LoadGroups(FileViewModel file)
        {
            try
            {
                var groupViewModels = new List<TagGroupViewModel>();

                foreach (var group in file.Groups)
                {
                    var groupPath = Path.Combine(_reportsDirectory, group);

                    if (File.Exists(groupPath))
                    {
                        var json = File.ReadAllText(groupPath);
                        var groupData = JsonConvert.DeserializeObject<TagGroupData>(json);

                        groupViewModels.Add(new TagGroupViewModel
                        {
                            GroupPath = group,
                            TagGroup = groupData.TagGroup,
                            GroupName = groupData.GroupName,
                            ErrorLevel = groupData.ErrorLevel,
                            TagErrorCount = groupData.TagErrorCount,
                            Tags = groupData.Tags,
                            ErrorInfo = $"Errors: {groupData.TagErrorCount}"
                        });
                    }
                }

                GroupsList.ItemsSource = groupViewModels;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading groups: {ex.Message}");
            }
        }

        private void GroupsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TagsList.ItemsSource = null;
            ErrorsList.ItemsSource = null;

            if (GroupsList.SelectedItem is TagGroupViewModel selectedGroup)
            {
                LoadTags(selectedGroup);
            }
        }

        private void LoadTags(TagGroupViewModel group)
        {
            var tagViewModels = group.Tags.Select(t => new TagViewModel
            {
                TagName = t.TagName,
                Errors = t.Errors,
                ErrorLevel = t.Errors.Count > 0 ? "All" : "None",
                ErrorInfo = $"Errors: {t.Errors.Count}"
            }).ToList();

            TagsList.ItemsSource = tagViewModels;
        }

        private void TagsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ErrorsList.ItemsSource = null;

            if (TagsList.SelectedItem is TagViewModel selectedTag)
            {
                ErrorsList.ItemsSource = selectedTag.Errors;
            }
        }
    }
}