/**
 * Raphael.Export https://github.com/ElbertF/Raphael.Export
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/mit-license.php
 *
 */

(function(R) {
	/**
	* Escapes string for XML interpolation
	* @param value string or number value to escape
	* @returns string escaped
	*/
	function escapeXML(s) {
		if ( typeof s === 'number' ) return s.toString();

		var replace = { '&': 'amp', '<': 'lt', '>': 'gt', '"': 'quot', '\'': 'apos' };

		for ( var entity in replace ) {
			s = s.replace(new RegExp(entity, 'g'), '&' + replace[entity] + ';');
		}

		return s;
	}

	/**
	* Generic map function
	* @param iterable the array or object to be mapped
	* @param callback the callback function(element, key)
	* @returns array
	*/
	function map(iterable, callback) {
		var mapped = new Array;

		for ( var i in iterable ) {
			if ( iterable.hasOwnProperty(i) ) {
				var value = callback.call(this, iterable[i], i);

				if ( value !== null ) mapped.push(value);
			}
		}

		return mapped;
	}

	/**
	* Generic reduce function
	* @param iterable array or object to be reduced
	* @param callback the callback function(initial, element, i)
	* @param initial the initial value
	* @return the reduced value
	*/
	function reduce(iterable, callback, initial) {
		for ( var i in iterable ) {
			if ( iterable.hasOwnProperty(i) ) {
				initial = callback.call(this, initial, iterable[i], i);
			}
		}

		return initial;
	}

	/**
	* Utility method for creating a tag
	* @param name the tag name, e.g., 'text'
	* @param attrs the attribute string, e.g., name1="val1" name2="val2"
	* or attribute map, e.g., { name1 : 'val1', name2 : 'val2' }
	* @param content the content string inside the tag
	* @returns string of the tag
	*/
	function tag(name, attrs, matrix, content) {
		if ( typeof content === 'undefined' || content === null ) {
			content = '';
		}

		if ( typeof attrs === 'object' ) {
			attrs = map(attrs, function(element, name) {
				if ( name === 'transform') return;
				if ((name === 'fill') || (name==='stroke')) {
					return name+'="' + hsb2Hex(element) + '"';
				} 
                if(name === 'stroke-dasharray')
                {
                    if((element == null)|| (element==undefined)||(element.length==0))
                    {
                        return;
                    }
                }
				return name + '="' + escapeXML(element) + '"';
			}).join(' ');
		}
		return '<' + name + ( matrix ? ' transform="matrix(' + matrix.toString().replace(/^matrix\(|\)$/g, '') + ')" ' : ' ' ) + attrs + '>' +  content + '</' + name + '>';
	}

	/**
	* @return object the style object
	*/
	function extractStyle(node) {
		//'text-anchor: middle; '
		return {
			font: {
				family: node.attrs.font.replace(/^.*?"(\w+)".*$/, '$1'),
				size:   typeof node.attrs['font-size'] === 'undefined' ? null : node.attrs['font-size'],
				anchor:   typeof node.attrs['text-anchor'] === 'undefined' ? 'middle' : node.attrs['text-anchor']
				}
			};
	}

	/**
	* @param style object from style()
	* @return string
	*/
	function styleToString(style) {
		//'text-anchor: middle; '
		// TODO figure out what is 'normal'
		return 'text-anchor:'+style.font.anchor+';font: normal normal normal 10px/normal ' + style.font.family + ( style.font.size === null ? '' : '; font-size: ' + style.font.size + 'px' );
	}

	/**
	* Computes tspan dy using font size. This formula was empircally determined
	* using a best-fit line. Works well in both VML and SVG browsers.
	* @param fontSize number
	* @return number
	*/
	function computeTSpanDy(fontSize, line, lines) {
		if ( fontSize === null ) fontSize = 11;
		var dy = fontSize * 4.5 / 13 * ( line - .2 - lines / 2 ) * 3.5;
		return dy;
	}

	var serializer = {
		'text': function(node) {
			style = extractStyle(node);

			//var tags = new Array;
            var tags = "";

            var tmp = node.attrs['text'];
            if(tmp != undefined)
            {
                var lines = tmp.split('\n');    
            
    			$.each(lines, function(i, line)
                {
					var fontsize= style.font.size;
					var text = escapeXML(line);
    					tags += tag(
    						'text',
    						reduce(
    							node.attrs,
    							function(initial, value, name) {
    								if ( name !== 'text' && name !== 'w' && name !== 'h' ) {
    									if ( name === 'font-size') value = value + 'px';
    
    									initial[name] = escapeXML(value.toString());
    								}
    
    								return initial;
    							},
    							{ style: styleToString(style) + ';' }
    							),
    						node.matrix,
    						tag('tspan', { dy: computeTSpanDy(fontsize, i + 1, lines.length) }, null, text)
    					);
                });
			}
            
			return tags;
		},
		'path' : function(node) {
			var initial = ( node.matrix.a === 1 && node.matrix.d === 1 ) ? {} : { 'transform' : node.matrix.toString() };

			return tag(
				'path',
				reduce(
					node.attrs,
					function(initial, value, name) {
						if ( name === 'path' ) name = 'd';

						initial[name] = value.toString();

						return initial;
					},
					{}
				),
				node.matrix
				);
		}
		/*,
		'circle': function(node){
			
		}*/		
		// Other serializers should go here
	};
	function hsb2Hex(hsb){
		if(hsb.indexOf('hsb')>=0)
		{
			hsb = hsb.match(/^hsb\s*\(\s*([0-9.]+)\s*,\s*([0-9.]+)\s*,\s*([0-9.]+)\s*\)$/);
			return Raphael.hsb2rgb(parseFloat(hsb[1]),parseFloat(hsb[2]),parseFloat(hsb[3])).hex;
		}
		return hsb;
	}
	R.fn.toSVG = function() {
		var
			paper   = this,
			restore = { svg: R.svg, vml: R.vml },
			svg     = '<svg style="overflow: hidden; position: relative;" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="' + paper.width + '" version="1.1" height="' + paper.height + '">'
			;

		R.svg = true;
		R.vml = false;

		for ( var node = paper.bottom; node != null; node = node.next ) {
			if ( node.node.style.display === 'none' ) continue;

			var attrs = '';

			
			// Use serializer
			if ( typeof serializer[node.type] === 'function' ) {
				svg += serializer[node.type](node);
				continue;
			}

			switch ( node.type ) {
				case 'image':
					attrs += ' preserveAspectRatio="none"';
					break;
			}

			for ( i in node.attrs ) {
				var name = i;

				switch ( i ) {
					case 'src':
						name = 'xlink:href';

						break;
					case 'transform':
						name = '';

						break;
				}

				if ( name ) 
				{
					if ((name === 'fill') || (name==='stroke')) {
						 attrs += ' '+name+'="' + hsb2Hex(node.attrs[i]) + '"';
					}
					else
					{
						attrs += ' ' + name + '="' + escapeXML(node.attrs[i].toString()) + '"';
					}
				}
			}

			svg += '<' + node.type + ' transform="matrix(' + node.matrix.toString().replace(/^matrix\(|\)$/g, '') + ')"' + attrs + '></' + node.type + '>';
		}

		svg += '</svg>';

		R.svg = restore.svg;
		R.vml = restore.vml;

		return svg;
	};
})(window.Raphael);
