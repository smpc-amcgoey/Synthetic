﻿using System;
using System.Collections;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitServices.Persistence;
using Autodesk.DesignScript.Runtime;

using revitView = Autodesk.Revit.DB.View;

using dynamoElem = Revit.Elements.Element;
using dynamoView = Revit.Elements.Views.View;

namespace Synthetic.Revit
{
    /// <summary>
    /// View Template Class
    /// </summary>
    public class ViewTemplate : dynamoView
    {
        /// <summary>
        /// Obtain the reference Element as a View
        /// </summary>
        internal Autodesk.Revit.DB.View InternalViewTemplate
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalViewTemplate; }
        }

        [IsVisibleInDynamoLibrary(false)]
        internal ViewTemplate(Autodesk.Revit.DB.View viewTemplate)
        {
            InternalViewTemplate = viewTemplate;
            this.InternalElementId = viewTemplate.Id;
            this.InternalUniqueId = viewTemplate.UniqueId;
        }

        /// <summary>
        /// Retreives all view templates in a document.
        /// </summary>
        /// <param name="doc">Autodesk.Revit.DB.Document object.</param>
        /// <returns name="templates">View templates</returns>
        /// <returns name="template names">Names of the view templates</returns>
        /// <returns name="template IDs">Element IDs of the view templates</returns>
        [MultiReturn(new[] { "templates", "template names", "template IDs" })]
        public static IDictionary GetViewTemplates(
            [DefaultArgument("Synthetic.Revit.Document.Current()")] Autodesk.Revit.DB.Document doc
            )
        {
            //Get Revit Document object
            //Document doc = DocumentManager.Instance.CurrentDBDocument;

            IList<ViewTemplate> templates = new List<ViewTemplate>();
            IList<string> templateNames = new List<string>();
            IList<ElementId> templateIds = new List<ElementId>();

            if (doc != null)
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                ICollection<Element> views = collector.OfClass(typeof(View)).ToElements();

                foreach (View view in views)
                {
                    if (view.IsTemplate)
                    {
                        templateNames.Add(view.Name);
                        templateIds.Add(view.Id);
                        templates.Add(new ViewTemplate(view));
                        //template.append(view.ToDSType(True))
                        //template.append(view)
                    }
                }
            }
            return new Dictionary<string, object>
            {
                {"templates", templates},
                {"template names", templateNames},
                {"template IDs", templateIds}
            };
        }

        /// <summary>
        /// Retreives all view templates in a document.
        /// </summary>
        /// <param name="name">Name of the view template.</param>
        /// <param name="doc">Autodesk.Revit.DB.Document object.</param>
        /// <returns name="templates">View templates</returns>
        /// <returns name="template names">Names of the view templates</returns>
        /// <returns name="template IDs">Element IDs of the view templates</returns>
        public static ViewTemplate GetViewTemplateByName( string name,
            [DefaultArgument("Synthetic.Revit.Document.Current()")] Autodesk.Revit.DB.Document doc
            )
        {
            ViewTemplate template = null;

            if (doc != null)
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                ICollection<Element> views = collector.OfClass(typeof(View)).ToElements();

                foreach (View view in views)
                {
                    if (view.IsTemplate)
                    {
                        if (view.Name == name)
                        {
                            return template = new ViewTemplate(view);
                        }
                    }
                }
            }
            return template;
        }

        /// <summary>
        /// Checks if the view is a view template.
        /// </summary>
        /// <param name="view">A Dynamo wrapped view.</param>
        /// <returns name="bool">True if the view is a template, otherwise false.</returns>
        public static bool IsViewTemplate (dynamoView view)
        {
            revitView rView = (revitView)view.InternalElement;
            return rView.IsTemplate;
        }

        /// <summary>
        /// Wraps a Revit view template for use in Dynamo.  Returns null if the view is not a template.
        /// </summary>
        /// <param name="viewTemplate">A Autodesk.Revit.DB.View view template.  Returns null if the view is not a template.</param>
        /// <returns name="View Template">A view template.</returns>
        public static ViewTemplate Wrap (revitView viewTemplate)
        {
            ViewTemplate vt = null;
            if (viewTemplate.IsTemplate)
            {
                vt = new ViewTemplate(viewTemplate);
            }
            return vt;
        }

        /// <summary>
        /// Wraps a Revit view template for use in Dynamo.  Returns null if the view is not a template.
        /// </summary>
        /// <param name="viewTemplate">A Autodesk.Revit.DB.View view template.  Returns null if the view is not a template.</param>
        /// <returns name="View Template">A view template.</returns>
        public static revitView Unwrap(ViewTemplate viewTemplate)
        {
            return viewTemplate.InternalViewTemplate;
        }

        /// <summary>
        /// Creates a string representation of the object.
        /// </summary>
        /// <returns nampe="string">A string representation of the object.</returns>
        public override string ToString()
        {
            return GetType().Name + "(Name = " + InternalViewTemplate.ViewName + " )";
        }
    }
}
