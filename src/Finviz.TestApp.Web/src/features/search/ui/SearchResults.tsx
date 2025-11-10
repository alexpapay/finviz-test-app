import { List, ListItemButton, ListItemText, Typography } from "@mui/material";
import { useSearchNodes } from "../model/useSearchNodes";

interface Props {
    query: string;
    onSelect: (id: number) => void;
}

export const SearchResults = ({ query, onSelect }: Props) => {
    const { data, isFetching } = useSearchNodes(query);

    if (!query.trim()) return null;

    if (isFetching) return <Typography variant="body2">Searching...</Typography>;

    if (!data || data.items.length === 0)
        return <Typography variant="body2">Nothing found</Typography>;

    return (
        <List dense>
            {data.items.map((item) => (
                <ListItemButton key={item.id} onClick={() => onSelect(item.id)}>
                    <ListItemText
                        primary={item.name}
                        secondary={item.fullPath}
                    />
                </ListItemButton>
            ))}
        </List>
    );
};
