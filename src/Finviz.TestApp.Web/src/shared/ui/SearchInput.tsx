import { TextField } from "@mui/material";

interface Props {
    value: string;
    onChange: (value: string) => void;
    placeholder?: string;
}

export const SearchInput = ({ value, onChange, placeholder }: Props) => (
    <TextField
        size="small"
        fullWidth
        variant="outlined"
        value={value}
        placeholder={placeholder ?? "Search..."}
        onChange={(e) => onChange(e.target.value)}
    />
);
