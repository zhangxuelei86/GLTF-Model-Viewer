﻿using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ImportedModelInfo
{
    public ImportedModelInfo(
        string fullFilePath,
        GltfObject gltfObject)
    {
        // Where were these files loaded from?
        this.BaseDirectoryPath = Path.GetDirectoryName(fullFilePath);

        // What's the name of the file itself?
        this.relativeLoadedFilePaths = new List<string>();
        this.relativeLoadedFilePaths.Add(Path.GetFileName(fullFilePath));

        // Note: At the time of writing, I'm unsure about what the URI property
        // might contain here for buffers and images given that the GLTF spec
        // says that it can be file URIs or data URIs and so what does the GLTF
        // reading code return to me in these cases?

        // I'm expected Uris like 
        //  foo.bin
        //  subfolder/foo.bin
        //  subfolder/bar/foo.bin

        // and will probably fail if I encounter something other than that.

        // Revisiting this in August 2020 the comment above came true in that
        // there are various files that I fail to open. Trying to correct
        // that with this code which "tries" to be more robust.
        List<string> definedUris = new List<string>();

        if (gltfObject.buffers != null)
        {
            definedUris.AddRange(
                gltfObject.buffers
                    .Where(b => !string.IsNullOrEmpty(b.uri) && File.Exists(Path.Combine(this.BaseDirectoryPath, b.uri)))
                    .Select(b => b.uri));
        }
        if (gltfObject.images != null)
        {
            definedUris.AddRange(
                gltfObject.images
                    .Where(i => !string.IsNullOrEmpty(i.uri) && File.Exists(Path.Combine(this.BaseDirectoryPath, i.uri)))
                    .Select(i => i.uri));
        }
        this.relativeLoadedFilePaths.AddRange(definedUris);

        this.GameObject = gltfObject.GameObjectReference;
    }
    public string BaseDirectoryPath { get; private set; }
    public IReadOnlyList<string> RelativeLoadedFilePaths => this.relativeLoadedFilePaths.AsReadOnly();
    public GameObject GameObject { get; set; }

    List<string> relativeLoadedFilePaths;
}