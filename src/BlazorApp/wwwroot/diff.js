function loadDiffGraph(graph) {
    // global object that will hold some config values
    let appStatus = {};
    appStatus.graph = {};
    console.log(graph);
    let cy = cytoscape({
        container: document.getElementById("cy"),

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
                    'label': 'data(counter)',
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
                        return "${contentFolder}/" + ele.data('id') + ".png"
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
                    'label': 'data(id)',
                    'background-image': "img/blackhole-bg.jpg",
                    'background-fit': 'contain',
                    'label': 'BlackHole'
                }
            },

            {
                selector: '.Added',
                style: {
                    'background-image': "img/plus.png"
                }
            },

            {
                selector: '.Removed',
                style: {
                    'background-image': "img/min.png"
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
            name: 'grid'
        },

        wheelSensitivity: 0.5
    });
}