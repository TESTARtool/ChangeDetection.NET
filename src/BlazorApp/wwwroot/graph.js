let cyMap = new Map();

function loadGraphCanvas(graph, elementId, dotNetHelper) {
    // this function should only be called once per view
    // since the user can show multiple graphs replace the entry
    // remove the known reference when an entry already exist.
    if (cyMap.has(elementId)) {
        cyMap.delete(elementId);
    }

    // global object that will hold some config values
    let savedLayout = 'cose-bilkent';
    if (localStorage.graphLayout) {
        savedLayout = localStorage.graphLayout;
    }

    let appStatus = {};
    appStatus.graph = {};

    let cy = cytoscape({
        container: document.getElementById(elementId),

        elements: JSON.parse(graph),

        style: [ // the stylesheet for the graph
            {
                selector: 'node',
                style: {
                    'background-color': '#F6EFF7',
                    'border-width': "1px",
                    'border-color': '#000000',
                    'label': '',
                    'color': '#5d574d',
                    'font-size': '0.4em'
                }
            },

            {
                selector: 'node[counter]',
                style: {
                    'label': 'data(counter)',
                }
            },

            {
                selector: ':parent',
                style: {
                    'background-opacity': 0.9,
                    'border-style': 'dashed',
                    'label': 'data(id)'
                }
            },

            {
                selector: 'edge',
                style: {
                    'width': 1,
                    'line-color': '#ccc',
                    'target-arrow-color': '#ccc',
                    'target-arrow-shape': 'triangle',
                    'curve-style': 'unbundled-bezier',
                    'text-rotation': 'autorotate',
                    'label': '',
                    'color': '#5d574d',
                    'font-size': '0.3em'
                }
            },
            {
                selector: 'edge[counter]',
                style: {
                    'label': 'data(uiLabel)',
                }
            },
            {
                selector: '.AbstractAction',
                style: {
                    'line-color': '#1c9099',
                    'target-arrow-color': '#1c9099'
                }
            },

            {
                selector: '.AbstractState',
                style: {
                    'background-color': '#1c9099',
                    'label': 'data(customLabel)'
                }
            },

            {
                selector: '.isInitial',
                style: {
                    'background-color': '#1c9099',
                    'width': '60px',
                    'height': '60px',
                    'border-color': '#000000'
                }
            },

            {
                selector: '.ConcreteState',
                style: {
                    'background-color': '#67A9CF',
                    'background-image': function (ele) {
                        return "data:image/png;base64," + ele.data('screenshot')
                    },
                    'background-fit': 'contain',
                    'label': 'data(customLabel)'
                }
            },

            {
                selector: '.ConcreteAction',
                style: {
                    'line-color': '#67A9CF',
                    'target-arrow-color': '#67A9CF'
                }
            },

            {
                selector: '.isAbstractedBy',
                style: {
                    'line-color': '#bdc9e1',
                    'target-arrow-color': '#bdc9e1',
                    'line-style': 'dashed',
                    'arrow-scale': 0.5,
                    'width': 0.5
                }
            },

            {
                selector: '.SequenceStep',
                style: {
                    'line-color': '#016450',
                    'target-arrow-color': '#016450'
                }
            },

            {
                selector: '.SequenceNode',
                style: {
                    'background-color': '#016450',
                    'label': 'data(customLabel)'
                }
            },

            {
                selector: '.FirstNode',
                style: {
                    'line-color': '#014636',
                    'target-arrow-color': '#014636'
                }
            },

            {
                selector: '.TestSequence',
                style: {
                    'background-color': '#014636',
                    'label': 'data(customLabel)'
                }
            },
            {
                selector: '.Accessed',
                style: {
                    'line-color': '#d0d1e6',
                    'target-arrow-color': '#d0d1e6',
                    'line-style': 'dashed',
                    'arrow-scale': 0.5,
                    'width': 0.5
                }
            },

            {
                selector: '.Widget',
                style: {
                    'background-color': '#e7298a',
                    'background-opacity': 0.8,
                    'label': 'data(customLabel)'
                }
            },

            {
                selector: '.isChildOf',
                style: {
                    'line-color': "#df65b0",
                    'target-arrow-color': '#df65b0'
                }
            },

            {
                selector: '.BlackHole',
                style: {
                    'background-color': '#000000',
                    'background-image': "img/blackhole-bg.jpg",
                    'background-fit': 'contain',
                    'label': 'BlackHole'
                }
            },

            {
                selector: '.New',
                style: {
                    'background-color': 'green',
                    'background-image': 'linear-gradient(green 1px, black 1px), linear-gradient(to right, green 1px, black 1px)',
                }
            },

            {
                selector: '.Removed',
                style: {
                    'background-color': 'red',
                    'background-image': 'repeating-linear-gradient(45deg, red 0, red 1px, black 0, black 50%)',
                }
            },

            {
                selector: '.UnvisitedAbstractAction',
                style: {
                    'line-color': "#1c9099",
                    'target-arrow-color': "#1c9099",
                    'line-style': 'dashed',
                    'width': 1
                }
            },

            {
                selector: '.no-label',
                style: {
                    'label': ''
                }
            },

            {
                selector: '.invisible',
                style: {
                    'display': 'none'
                }
            },

            {
                selector: '.dim',
                style: {
                    'line-color': "#FFFFFF",
                    'target-arrow-color': "#FFFFFF",
                    'background-color': '#FFFFFF',
                    'border-color': '#FFFFFF',
                    'background-image-opacity': 0.05
                }
            },
            {
                selector: '.leaves',
                style: {
                    'width': '60px',
                    'height': '60px',
                    'border-width': '2px'
                }
            },
            {
                selector: '.errorState',
                style: {
                    'border-color': '#FF0000',
                    'line-color': '#FF0000'
                }
            },
            {
                selector: '.selected-node',
                style: {
                    'width': '50px',
                    'height': '50px',
                    'border-color': '#4be2ff'
                }
            },
            {
                selector: '.selected-node-animated',
                style: {
                    'width': '50px',
                    'height': '50px',
                    'border-color': '#4be2ff',
                    'transition-property': 'width height border-color',
                    'transition-duration': '0.5s',
                    'transition-timing-function': 'ease-out-sine'
                }
            },
            {
                selector: '.selected-initial-node',
                style: {
                    'width': '60px',
                    'height': '60px',
                    'border-color': '#4be2ff'
                }
            },
            {
                selector: '.selected-initial-node-animated',
                style: {
                    'width': '60px',
                    'height': '60px',
                    'border-color': '#4be2ff',
                    'transition-property': 'width height border-color',
                    'transition-duration': '0.5s',
                    'transition-timing-function': 'ease-out-sine'
                }
            },
            {
                selector: '.connected-concrete-state-node',
                style: {
                    'width': '50px',
                    'height': '50px',
                    'border-color': '#3255ff'
                }
            },
            {
                selector: '.selected-edge',
                style: {
                    'line-color': "#4be2ff",
                    'target-arrow-color': "#4be2ff",
                    'line-style': 'solid',
                    'width': 2,
                    'font-size': '8px',
                    'font-weight': 'bold'
                }
            },
            {
                selector: '.mouse-over-concrete-action',
                style: {
                    'label': 'data(Desc)'
                }
            }
        ],

        layout: {
            name: savedLayout
        },
    });

    let showLabels = document.getElementById("show-labels");
    if (showLabels != null) {
        showLabels.addEventListener("change", function () {
            if (showLabels.checked) {
                cy.$('.no-label').removeClass('no-label');
            }
            else {
                cy.$('node').addClass('no-label');
                cy.$('edge').addClass('no-label');
            }
        });
    }

    // when nodes get clicked, we need to open the side bar
    cy.on('tap', function (evt) {
        dotNetHelper.invokeMethodAsync('UpdateSelectedElement', evt.target.data('id'));
    });

    function initStats() {
        let div = document.getElementById('stats-abstract-states');
        if (div != null) {
            let text = document.createTextNode(appStatus.nrOfAbstractStates);
            div.append(text);
        }

        div = document.getElementById('stats-abstract-actions');
        if (div != null) {
            let text = document.createTextNode(appStatus.nrOfAbstractActions);
            div.append(text);
        }

        div = document.getElementById('stats-concrete-states');
        if (div != null) {
            let text = document.createTextNode(appStatus.nrOfConcreteStates);
            div.append(text);
        }

        div = document.getElementById('stats-concrete-actions');
        if (div != null) {
            let text = document.createTextNode(appStatus.nrOfConcreteActions);
            div.append(text);
        }
    }
    function initLayers() {
        appStatus.nrOfAbstractStates = cy.$('node.AbstractState').size();
        appStatus.nrOfConcreteStates = cy.$('node.ConcreteState').size();
        appStatus.nrOfSequenceNodes = cy.$('node.SequenceNode').size();
        appStatus.nrOfAbstractActions = cy.$('edge.AbstractAction').size();
        appStatus.nrOfConcreteActions = cy.$('edge.ConcreteAction').size();
        appStatus.nrOfUnvisitedAbstractActions = cy.$('edge.UnvisitedAbstractAction').size();
        appStatus.abstractLayerPresent = appStatus.nrOfAbstractStates > 0;
        appStatus.concreteLayerPresent = appStatus.nrOfConcreteStates > 0;
        appStatus.sequenceLayerPresent = appStatus.nrOfSequenceNodes > 0;
        appStatus.nrOfLayersPresent = 0;

        // ready several toggle buttons
        // abstract layer toggle
        let abstractLayerToggle = document.getElementById("toggle-abstract-layer");
        if (abstractLayerToggle != null) {
            if (appStatus.abstractLayerPresent) {
                appStatus.nrOfLayersPresent++;
                abstractLayerToggle.checked = true;
                abstractLayerToggle.addEventListener("change", (e) => {
                    if (abstractLayerToggle.checked) {
                        cy.$('node.AbstractState').union(cy.$('node.BlackHole')).union(cy.$('node.AbstractState').parent()).removeClass("invisible");
                    }
                    else {
                        cy.$('node.AbstractState').union(cy.$('node.BlackHole')).union(cy.$('node.AbstractState').parent()).addClass("invisible");
                    }
                });
            }
            else {
                abstractLayerToggle.checked = false;
                abstractLayerToggle.disabled = true;
            }
        }

        // concrete layer toggle
        let concreteLayerToggle = document.getElementById("toggle-concrete-layer");
        if (concreteLayerToggle != null) {
            if (appStatus.concreteLayerPresent) {
                appStatus.nrOfLayersPresent++;
                concreteLayerToggle.checked = true;
                concreteLayerToggle.addEventListener("change", (e) => {
                    if (concreteLayerToggle.checked) {
                        cy.$('node.ConcreteState').union(cy.$('node.ConcreteState').parent()).removeClass("invisible");
                    }
                    else {
                        cy.$('node.ConcreteState').union(cy.$('node.ConcreteState').parent()).addClass("invisible");
                    }
                });
            }
            else {
                concreteLayerToggle.checked = false;
                concreteLayerToggle.disabled = true;
            }
        }

        // sequence layer toggle
        let sequenceLayerToggle = document.getElementById("toggle-sequence-layer");
        if (sequenceLayerToggle != null) {
            if (appStatus.sequenceLayerPresent) {
                appStatus.nrOfLayersPresent++;
                sequenceLayerToggle.checked = true;
                sequenceLayerToggle.addEventListener("change", (e) => {
                    if (sequenceLayerToggle.checked) {
                        cy.$('node.SequenceNode').union(cy.$('node.SequenceNode').parent()).removeClass("invisible");
                    }
                    else {
                        cy.$('node.SequenceNode').union(cy.$('node.SequenceNode').parent()).addClass("invisible");
                    }
                });
            }
            else {
                sequenceLayerToggle.checked = false;
                sequenceLayerToggle.disabled = true;
            }
        }
        // toggle for edges between the layers
        let interLayerEdgesToggle = document.getElementById("toggle-layer-transitions");
        if (interLayerEdgesToggle != null) {
            if (appStatus.nrOfLayersPresent > 1 && appStatus.concreteLayerPresent) {
                interLayerEdgesToggle.checked = true;
                interLayerEdgesToggle.addEventListener("change", (e) => {
                    if (interLayerEdgesToggle.checked) {
                        cy.$('edge.isAbstractedBy').union(cy.$('edge.Accessed')).removeClass("invisible");
                    }
                    else {
                        cy.$('edge.isAbstractedBy').union(cy.$('edge.Accessed')).addClass("invisible");
                    }
                });
            }
            else {
                interLayerEdgesToggle.checked = false;
                interLayerEdgesToggle.disabled = true;
            }
        }
    }

    cy.ready(function (event) {
        initLayers();
        initStats();

        // highlight the leaves, which in this case will be the root of the widget tree
        cy.$(".Widget").leaves().addClass("leaves");
        cy.$(".Widget").forEach(
            (w) => w.data("customLabel", w.data("Role") + "-" + w.data("counter"))
        );

        // concrete state:
        cy.$(".ConcreteState").forEach(
            (w) => {
                if (w.data('oracleVerdictCode') && ["2", "3"].includes(w.data('oracleVerdictCode'))) {
                    w.addClass('errorState');
                }
            }
        );

        // add a mouseover event to the concrete actions
        cy.$(".ConcreteAction").on('mouseover', function (mouseoverEvent) {
            mouseoverEvent.target.addClass("mouse-over-concrete-action");
        }).
            on('mouseout', function (mouseoutEvent) {
                mouseoutEvent.target.removeClass("mouse-over-concrete-action");
            });

        // add a highlight when mousing over nodes
        cy.$('node').on('mouseover', (nodeMouseOverEvent) => {
            // if there is currently a selected node or edge, we don't highlight, as there will already be a highlight in place
            //if ("selectedNode" in appStatus.graph && appStatus.graph.selectedNode != null) return;
            //if ("selectedEdge" in appStatus.graph && appStatus.graph.selectedEdge != null) return;
            if (nodeMouseOverEvent.target.is(':parent')) return;
            nodeMouseOverEvent.target.addClass(nodeMouseOverEvent.target.hasClass('isInitial') ? 'selected-initial-node' : 'selected-node');

            // if the node is a sequence node, we want to also highlight the corresponding concrete action node
            if (nodeMouseOverEvent.target.hasClass('SequenceNode')) {
                cy.$(nodeMouseOverEvent.target).outgoers('.ConcreteState').addClass('selected-node');
            }
        });
        cy.$('node').on('mouseout', (nodeMouseOutEvent) => {
            // no action taken if there is a selected node or edge
            //if ("selectedNode" in appStatus.graph && appStatus.graph.selectedNode != null) return;
            //if ("selectedEdge" in appStatus.graph && appStatus.graph.selectedEdge != null) return;
            if (nodeMouseOutEvent.target.is(':parent')) return;

            if (nodeMouseOutEvent.target.hasClass('selected-node')) {
                nodeMouseOutEvent.target.removeClass('selected-node');
            }
            if (nodeMouseOutEvent.target.hasClass('selected-initial-node')) {
                nodeMouseOutEvent.target.removeClass('selected-initial-node');
            }

            if (nodeMouseOutEvent.target.hasClass('SequenceNode')) {
                cy.$(nodeMouseOutEvent.target).outgoers('.ConcreteState').removeClass('selected-node');
            }
        });

        cy.$('edge').on('mouseover', (edgeMouseoverEvent) => {
            // if there is currently a selected node or edge, we don't highlight, as there will already be a highlight in place
            //if ("selectedNode" in appStatus.graph && appStatus.graph.selectedNode != null) return;
            //if ("selectedEdge" in appStatus.graph && appStatus.graph.selectedEdge != null) return;

            if (edgeMouseoverEvent.target.hasClass('isAbstractedBy') || edgeMouseoverEvent.target.hasClass('Accessed')) return;
            edgeMouseoverEvent.target.addClass('selected-edge');

            // we also want to highlight the concrete action if
            if (edgeMouseoverEvent.target.hasClass('SequenceStep')) {
                edgeMouseoverEvent.target.source().outgoers('.ConcreteState').connectedEdges('.ConcreteAction').filter((element) =>
                    element.data('uid') == edgeMouseoverEvent.target.data('concreteActionUid')).addClass('selected-edge').addClass('mouse-over-concrete-action');
            }
        });
        cy.$('edge').on('mouseout', (edgeMouseoutEvent) => {
            // no action taken if there is a selected node or edge
            if ("selectedNode" in appStatus.graph && appStatus.graph.selectedNode != null) return;
            if ("selectedEdge" in appStatus.graph && appStatus.graph.selectedEdge != null) return;

            if (edgeMouseoutEvent.target.hasClass('isAbstractedBy') || edgeMouseoutEvent.target.hasClass('Accessed')) return;
            edgeMouseoutEvent.target.removeClass('selected-edge');

            if (edgeMouseoutEvent.target.hasClass('SequenceStep')) {
                edgeMouseoutEvent.target.source().outgoers('.ConcreteState').connectedEdges('.ConcreteAction').filter((element) =>
                    element.data('uid') == edgeMouseoutEvent.target.data('concreteActionUid')).removeClass('selected-edge').removeClass('mouse-over-concrete-action');
            }
        });

        // legend boxes
        let abstractStateLegendBox = document.getElementById("legend-abstract-state");
        abstractStateLegendBox.addEventListener('click', () => {
            let initialNodes = cy.$('node.isInitial');
            let abstractStateNodes = cy.$('node.AbstractState').difference(initialNodes);
            abstractStateNodes.addClass('selected-node-animated');
            initialNodes.addClass('selected-initial-node-animated');
            setTimeout(() => {
                abstractStateNodes.removeClass('selected-node-animated');
                initialNodes.removeClass('selected-initial-node-animated');
            }, 1000);
        });

        let concreteStateLegendBox = document.getElementById("legend-concrete-state");
        concreteStateLegendBox.addEventListener('click', () => {
            let concreteStates = cy.$('node.ConcreteState');
            concreteStates.addClass('selected-node-animated');
            setTimeout(() => concreteStates.removeClass('selected-node-animated'), 1000);
        });
    });

    cyMap.set(elementId, cy);
}

function changeLayout(cyId, newLayout) {
    if (cyMap.has(cyId)) {
        let cy = cyMap.get(cyId);

        localStorage.setItem("graphLayout", newLayout);
        cy.layout({
            name: newLayout,
            animate: 'end',
            animationEasing: 'ease-out',
            animationDuration: 1000
        }).run();
    }
}

function showAllElements(cyId) {
    if (cyMap.has(cyId)) {
        let cy = cyMap.get(cyId);

        cy.$('.invisible').removeClass('invisible');
        cy.$('.dim').removeClass('dim');

        let abstractLayerToggle = document.getElementById("toggle-abstract-layer");
        if (abstractLayerToggle != null) {
            abstractLayerToggle.checked = true;
        }

        let concreteLayerToggle = document.getElementById("toggle-concrete-layer");
        if (concreteLayerToggle != null) {
            concreteLayerToggle.checked = true;
        }

        let sequenceLayerToggle = document.getElementById("toggle-sequence-layer");
        if (sequenceLayerToggle != null) {
            sequenceLayerToggle.checked = true;
        }

        let interLayerEdgesToggle = document.getElementById("toggle-layer-transitions");
        if (interLayerEdgesToggle != null) {
            interLayerEdgesToggle.checked = true;
        }

        initLayers();
    }
}

function traceSequence(cyId, selectedId) {
    if (cyMap.has(cyId)) {
        let cy = cyMap.get(cyId);
        let targetNode = cy.$(selectedId);
        // first, get all the nodes and edges in the sequence
        let sequenceElements = targetNode.successors('.SequenceNode, .SequenceStep, .FirstNode');
        // add the targetnode itself and the parents
        sequenceElements = sequenceElements.union(targetNode);

        // now get all the corresponding elements on the concrete layer
        let concreteStateNodes = sequenceElements.nodes('.SequenceNode').outgoers('.ConcreteState');
        let accessedEdges = sequenceElements.nodes('.SequenceNode').outgoers('.Accessed');
        let concreteActionNodes = concreteStateNodes.connectedEdges('.ConcreteAction');

        // for each sequence step, fetch the corresponding concrete action
        let selectedConcreteActionNodes = [];
        sequenceElements.edges('.SequenceStep').forEach(
            (edge) => concreteActionNodes.forEach(
                (concreteActionNode) => {
                    if (concreteActionNode.data('uid') == edge.data('concreteActionUid')) {
                        selectedConcreteActionNodes.push(concreteActionNode);
                    }
                }
            )
        );

        sequenceElements = sequenceElements.union(concreteStateNodes).union(accessedEdges).union(selectedConcreteActionNodes);

        // get all the corresponding elements on the abstract layer
        let abstractStateNodes = sequenceElements.nodes('.ConcreteState').outgoers('.AbstractState');
        let abstractionEdges = sequenceElements.nodes('.ConcreteState').outgoers('.isAbstractedBy');
        let abstractActionNodes = abstractStateNodes.connectedEdges('.AbstractAction');

        // next, for each concrete action in the neighborhoor, attempt to fetch the corresponding abstract action
        let selectedAbstractActionNodes = [];
        sequenceElements.edges('.ConcreteAction').forEach(
            (edge) => abstractActionNodes.forEach((abstractActionNode) => {
                if (abstractActionNode.data('concreteActionIds').indexOf(edge.data('actionId')) != -1) {
                    selectedAbstractActionNodes.push(abstractActionNode);
                }
            })
        );

        // now join the nodes
        sequenceElements = sequenceElements.union(abstractStateNodes).union(selectedAbstractActionNodes).union(abstractionEdges);

        // add the parents
        sequenceElements = sequenceElements.union(sequenceElements.parent());
        cy.$("*").difference(sequenceElements).addClass("invisible");
    }
}

function tracePath(cyId, selectedId) {
    if (cyMap.has(cyId)) {
        let cy = cyMap.get(cyId);
        let targetNode = cy.$(selectedId);

        // get the sequence nodes that accessed this node
        let sequenceNodes = cy.$(targetNode).incomers(".SequenceNode, .Accessed");
        // get the complete list of sequence nodes that let to them
        let predecessorNodes = sequenceNodes.predecessors();
        // next get the concrete state nodes that were accessed by all these sequence nodes
        let concreteStateNodes = predecessorNodes.outgoers();
        // then, we have to get the concrete action nodes that correspond with the sequence steps
        let concreteActionUids = predecessorNodes.filter((element) => element.hasClass('SequenceStep')).map((element) =>
            element.data('concreteActionUid'));
        // now fetch the edges matching the collected ids
        let concreteActions = concreteStateNodes.connectedEdges('.ConcreteAction').filter((element) =>
            concreteActionUids.includes(element.data('uid')));
        let allElements = sequenceNodes.union(predecessorNodes).union(concreteStateNodes).union(targetNode).union(concreteActions);
        // add the parent nodes, if there are any
        allElements = allElements.union(cy.$(allElements).parent());
        cy.$("*").difference(allElements).addClass("invisible");
    }
}

function hideElement(cyId, selectedId) {
    if (cyMap.has(cyId)) {
        let cy = cyMap.get(cyId);
        let targetNode = cy.$(selectedId);

        targetNode.addClass("invisible");
    }
}

function highlightElement(cyId, selectedId) {
    if (cyMap.has(cyId)) {
        let cy = cyMap.get(cyId);
        let targetNode = cy.$(selectedId);

        // we want to select the clicked node and its neighborhood
        let allNodes = cy.$(targetNode).closedNeighborhood();

        // next, in the case of a concrete action or an abstract action, we want to also fetch their concrete and
        // abstract counterparts for the neighborhood
        if (targetNode.hasClass('ConcreteState')) {
            // get the connect abstract states and their connected abstract actions
            let abstractStateNodes = allNodes.nodes('.ConcreteState').outgoers('.AbstractState');
            let abstractionEdges = allNodes.nodes('.ConcreteState').outgoers('.isAbstractedBy');
            let abstractActionNodes = abstractStateNodes.connectedEdges('.AbstractAction');

            // next, for each concrete action in the neighborhoor, attempt to fetch the corresponding abstract action
            let selectedAbstractActionNodes = [];
            allNodes.edges('.ConcreteAction').forEach(
                (edge) => abstractActionNodes.forEach(
                    (abstractActionNode) => {
                        if (abstractActionNode.data('concreteActionIds').indexOf(edge.data('actionId')) != -1) {
                            selectedAbstractActionNodes.push(abstractActionNode);
                        }
                    }
                )
            );

            // now join the nodes
            allNodes = allNodes.union(abstractStateNodes).union(selectedAbstractActionNodes).union(abstractionEdges);
        }

        // we also need to select the parent nodes in case of a compound graph.
        allNodes = allNodes.union(allNodes.parent());
        cy.$("*").difference(allNodes).addClass("invisible");
    }
}