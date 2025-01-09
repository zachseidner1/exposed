shader_type canvas_item;

uniform vec2 size = vec2(32.0, 28.0);
// If you intend to read from mipmaps with `textureLod()` LOD values greater than `0.0`,
// use `filter_nearest_mipmap` instead. This shader doesn't require it.
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

const mat4 bayerIndex = mat4(
    vec4(00.0/16.0, 12.0/16.0, 03.0/16.0, 15.0/16.0),
    vec4(08.0/16.0, 04.0/16.0, 11.0/16.0, 07.0/16.0),
    vec4(02.0/16.0, 14.0/16.0, 01.0/16.0, 13.0/16.0),
    vec4(10.0/16.0, 06.0/16.0, 09.0/16.0, 05.0/16.0));

void fragment() {
		vec4 current_color = textureLod(screen_texture, SCREEN_UV, 0.0);
		float brightness = current_color.r;
		int col = int(FRAGCOORD.x) % 4;
		int row = int(FRAGCOORD.y) % 4;
		float offset = bayerIndex[col][row];
		// gamma correction
    	current_color = vec4(pow(current_color.rgb,vec3(2.2)) - 0.004,current_color.a);

		
        //COLOR = current_color;
		COLOR = vec4(
			step(offset, current_color.r),
			step(offset, current_color.g),
			step(offset, current_color.b),
			current_color.a
		);
}