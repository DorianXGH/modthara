﻿using System.IO.Compression;

using Modthara.Lari;

namespace Modthara.Manager;

/// <summary>
/// Manages mod packages in a directory.
/// </summary>
public interface IModsService
{
    /// <summary>
    /// Gets a read-only list of cached mod packages.
    /// </summary>
    /// <value>An <see cref="IReadOnlyList{ModPackage}"/> containing the mod packages.</value>
    IReadOnlyList<ModPackage> ModPackages { get; }

    string Path { get; set; }

    /// <summary>
    /// Loads packages from the specified path to <see cref="ModPackages"/>.
    /// </summary>
    /// <param name="onException">
    /// An optional action to handle exceptions that occur during the loading process. The first parameter is the index of the mod package, and the second parameter is the exception.
    /// </param>
    /// <param name="onPackageRead">
    /// An optional action to handle each successfully read mod package. The first parameter is the index of the mod package, and the second parameter is the mod package.
    /// </param>
    Task LoadModPackagesAsync(Action<int, Exception>? onException = null,
        Action<int, ModPackage>? onPackageRead = null);

    /// <summary>
    /// Retrieves mod packages and identifies missing mods from the provided mod settings.
    /// </summary>
    /// <param name="modSettings">
    /// The mod settings containing the mods to check.
    /// </param>
    /// <returns>
    /// A tuple containing two collections:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <see cref="IEnumerable{ModPackage}"/> of found mod packages.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <see cref="IEnumerable{Module}"/> of missing mods.
    /// </description>
    /// </item>
    /// </list>
    /// </returns>
    ValueTask<(IEnumerable<ModPackage> foundMods, IEnumerable<Module> missingMods)> GetModsFromModSettingsAsync(
        ModSettings modSettings);

    /// <summary>
    /// Retrieves mod packages and identifies missing mods from the provided order entries.
    /// </summary>
    /// <param name="orderEntries">
    /// The order entries containing the mods to check.
    /// </param>
    /// <returns>
    /// A tuple containing two collections:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <see cref="IEnumerable{ModPackage}"/> of found mod packages.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <see cref="IEnumerable{ModOrderEntry}"/> of missing mods.
    /// </description>
    /// </item>
    /// </list>
    /// </returns>
    ValueTask<(IEnumerable<ModPackage> foundMods, IEnumerable<ModOrderEntry> missingMods)> GetModsFromOrderAsync(
        IAsyncEnumerable<ModOrderEntry> orderEntries);

    /// <summary>
    /// Finds and returns a collection of duplicate mod packages.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of tuples, where each tuple contains two <see cref="ModPackage"/> objects
    /// that are considered duplicates.
    /// </returns>
    IEnumerable<(ModPackage, ModPackage)> FindDuplicateModPackages();

    /// <summary>
    /// Imports a mod package by copying it to the destination path and adding it to <see cref="ModPackages"/>.
    /// </summary>
    /// <param name="modPackage">
    /// The mod package to be imported.
    /// </param>
    Task ImportModPackageAsync(ModPackage modPackage);

    /// <summary>
    /// Imports mod packages from a zip archive by extracting and reading each package, then adding it to the collection of mod packages.
    /// </summary>
    /// <param name="zipArchive">
    /// The <see cref="ZipArchive"/> containing the mod packages.
    /// </param>
    /// <param name="onException">
    /// An optional action to handle exceptions that occur during the import process.
    /// </param>
    /// <param name="onPackageRead">
    /// An optional action to handle each successfully read mod package. The first parameter is the index of the mod package, and the second parameter is the mod package.
    /// </param>
    Task ImportArchivedModPackagesAsync(ZipArchive zipArchive, Action<Exception>? onException = null,
        Action<int, ModPackage>? onPackageRead = null);

    /// <summary>
    /// Enables the specified mod package by changing its file extension and updating its flags.
    /// </summary>
    /// <param name="modPackage">
    /// The mod package to enable.
    /// </param>
    void EnableModPackage(ModPackage modPackage);

    /// <summary>
    /// Disables the specified mod package by changing its file extension and updating its flags.
    /// </summary>
    /// <param name="modPackage">
    /// The mod package to disable.
    /// </param>
    void DisableModPackage(ModPackage modPackage);

    /// <summary>
    /// Deletes the specified mod package by removing its directory and removing it from the collection.
    /// </summary>
    /// <param name="modPackage">
    /// The mod package to delete.
    /// </param>
    void DeleteModPackage(ModPackage modPackage);
}
