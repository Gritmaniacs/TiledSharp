/* Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
 * Licensed under the Apache License, Version 2.0
 * http://www.apache.org/licenses/LICENSE-2.0 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;

namespace TiledSharp
{
    // TODO: The design here is all wrong. A Tileset should be a list of tiles,
    //       it shouldn't force the user to do so much tile ID management

    public class TmxTemplateGroup : TmxDocument, ITmxElement
    {
        public int FirstTid { get; private set; }
        public string Name { get; private set; }
        public Dictionary<int, TmxTemplate> Templates { get; private set; }

        // TSX file constructor
        public TmxTemplateGroup(XContainer xDoc, string tmxDir) :
            this(xDoc.Element("templategroup"), tmxDir)
        { }

        // TMX tileset element constructor
        public TmxTemplateGroup(XElement xTileset, string tmxDir = "")
        {
            var xFirstTid = (int?)xTileset.Attribute("firsttid") ?? 0;
            var source = (string)xTileset.Attribute("source");

            if (source != null)
            {
                // Prepend the parent TMX directory if necessary
                source = Path.Combine(tmxDir, source);

                FirstTid = xFirstTid;

                var xTemplateGroup = ReadXml(source);
                var templateGroup = new TmxTemplateGroup(xTemplateGroup, TmxDirectory);
                Name = templateGroup.Name;
                Templates = templateGroup.Templates;
            }
            else
            {
                Templates = new Dictionary<int, TmxTemplate>();
                Name = (string)xTileset.Attribute("name");

                foreach (var xTemplate in xTileset.Elements("template"))
                {
                    var newTemplate = new TmxTemplate(xTemplate);
                    Templates.Add(newTemplate.Id, newTemplate);
                }
            }
        }
    }

    public class TmxTemplate : ITmxElement
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public Dictionary<int, TmxTemplateObject> Objects { get; private set; }

        public TmxTemplate(XElement xFrame)
        {
            Id = (int)xFrame.Attribute("id");
            Name = (string)xFrame.Attribute("name");

            Objects = new Dictionary<int, TmxTemplateObject>();

            foreach (var xObject in xFrame.Elements("object"))
            {
                var newObject = new TmxTemplateObject(xObject);
                Objects.Add(newObject.Gid, newObject);
            }
        }
    }

    public class TmxTemplateObject
    {
        public int Gid { get; private set; }
        public string Type { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }
        public PropertyDict Properties { get; private set; }

        public TmxTemplateObject(XElement xFrame)
        {
            Gid = xFrame.Attribute("gid") != null ? (int)xFrame.Attribute("gid") : 0;
            Type = (string)xFrame.Attribute("type");
            Width = (double)xFrame.Attribute("width");
            Height = (double)xFrame.Attribute("height");

            Properties = new PropertyDict(xFrame.Element("properties"));
        }
    }
}
